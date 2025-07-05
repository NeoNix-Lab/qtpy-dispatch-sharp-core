import socket
import json
import struct
from typing import Optional, Literal


class StreamMessage:
    """
    Represents a length-prefixed JSON message with a command and payload.
    """
    def __init__(self, command: str, payload: str):
        self.Command = command
        self.Payload = payload

    def to_bytes(self) -> bytes:
        """
        Serialize the message to bytes with a 4-byte little-endian length prefix.
        """
        # Convert object fields to JSON bytes
        raw = json.dumps(self.__dict__).encode('utf-8')
        # Pack the length of the JSON payload into 4 bytes (little-endian)
        length_prefix = struct.pack('<I', len(raw))
        return length_prefix + raw


class SocketClientService:
    """
    A TCP client that sends and receives length-prefixed JSON messages.

    Attributes:
        host (str): The remote hostname or IP address.
        port (int): The remote TCP port.
        timeout (Optional[float]): Socket timeout in seconds for connect/send/receive.
        status (Literal['disconnected', 'connected']): Current connection state.
        sock (Optional[socket.socket]): Underlying TCP socket.
    """

    def __init__(self, host: str, port: int, timeout: Optional[float] = None):
        self.host = host
        self.port = port
        self.timeout = timeout
        self.status: Literal["disconnected", "connected"] = "disconnected"
        self.sock: Optional[socket.socket] = None

    def connect(self) -> None:
        """
        Establishes a TCP connection to the configured host and port.
        Applies the timeout to both connect and subsequent socket operations.
        Raises TimeoutError if connection attempt times out.
        """
        try:
            if self.timeout is not None:
                # Create connection with timeout
                self.sock = socket.create_connection(
                    (self.host, self.port),
                    timeout=self.timeout
                )
                # Use the same timeout for all subsequent I/O operations
                self.sock.settimeout(self.timeout)
            else:
                self.sock = socket.create_connection((self.host, self.port))
            self.status = "connected"
        except socket.timeout:
            # Close socket on timeout and update status
            if self.sock:
                self.sock.close()
            self.status = "disconnected"
            raise TimeoutError(f"Connect timed out after {self.timeout} seconds")

    async def send(self, command: str, payload: str) -> Optional[dict]:
        """
        Sends a length-prefixed JSON message and waits for a JSON response.
        Closes the connection and raises TimeoutError if any operation times out.

        Args:
            command (str): The command string to include in the message.
            payload (str): The payload string to include in the message.

        Returns:
            dict or None: Parsed JSON response, or None if no data received.
        """
        if not self.sock:
            self.status = "disconnected"
            raise ConnectionError("Socket is not connected.")

        # Build the message bytes
        message_bytes = StreamMessage(command, payload).to_bytes()

        try:
            # Send the framed message
            self.sock.sendall(message_bytes)

            # Read the 4-byte length prefix
            len_prefix = self.sock.recv(4)
            if not len_prefix:
                return None
            msg_len = struct.unpack('<I', len_prefix)[0]

            # Read the full payload in a loop until complete
            data = b''
            while len(data) < msg_len:
                chunk = self.sock.recv(msg_len - len(data))
                if not chunk:
                    break
                data += chunk

            # Decode and parse JSON response
            return json.loads(data.decode('utf-8'))

        except socket.timeout:
            # On timeout, close the socket and update status
            self.close()
            raise TimeoutError(f"Operation timed out after {self.timeout} seconds")

    def close(self) -> None:
        """
        Gracefully shuts down and closes the TCP connection.
        Updates the connection status to 'disconnected'.
        """
        if self.sock:
            try:
                # Shutdown both send and receive
                self.sock.shutdown(socket.SHUT_RDWR)
            except Exception:
                pass
            finally:
                self.sock.close()
        self.status = "disconnected"
