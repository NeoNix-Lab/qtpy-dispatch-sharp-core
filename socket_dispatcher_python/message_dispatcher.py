from typing import Dict

from .message_envelope import MessageEnvelope


class MessageDispatcher:
    """Routes incoming JSON messages to the registered envelopes."""

    def __init__(self) -> None:
        self._envelopes: Dict[str, MessageEnvelope] = {}

    def register(self, envelope: MessageEnvelope) -> None:
        if envelope.name in self._envelopes:
            raise ValueError(f"Envelope for '{envelope.name}' already registered")
        self._envelopes[envelope.name] = envelope

    def dispatch(self, json_str: str) -> None:
        envelope = MessageEnvelope.from_json(json_str)
        if envelope.name in self._envelopes:
            registered = self._envelopes[envelope.name]
            envelope.on_received = registered.on_received
            envelope.invoke()
