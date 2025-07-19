import json
from typing import Any, Dict, Callable, Optional

from .dynamic_message import DynamicMessage


class MessageEnvelope:
    """Wraps a :class:`DynamicMessage` and stores a callback to invoke."""

    def __init__(self, name: str, data: Dict[str, Any], on_received: Optional[Callable[[DynamicMessage], None]] = None) -> None:
        self.name = name
        self.message = DynamicMessage(name, data)
        self.on_received = on_received

    @classmethod
    def create(cls, name: str, data: Dict[str, Any], on_received: Callable[[DynamicMessage], None]) -> "MessageEnvelope":
        return cls(name, data, on_received)

    @classmethod
    def from_json(cls, json_str: str) -> "MessageEnvelope":
        obj = json.loads(json_str)
        name = obj.get("name") or obj.get("Name")
        if "data" in obj:
            data = obj["data"]
        elif "Data" in obj:
            data = obj["Data"]
        else:
            obj.pop("name", None)
            obj.pop("Name", None)
            data = obj
        return cls(name, data)

    def to_json(self) -> str:
        return json.dumps({"name": self.name, "data": self.message.data})

    def invoke(self) -> None:
        if self.on_received:
            self.on_received(self.message)
