import json
import struct


class StreamMessage:
    """Represents a length-prefixed JSON message with a command and payload."""

    def __init__(self, command: str, payload: str) -> None:
        self.Command = command
        self.Payload = payload

    def to_bytes(self) -> bytes:
        raw = json.dumps(self.__dict__).encode("utf-8")
        length_prefix = struct.pack("<I", len(raw))
        return length_prefix + raw
