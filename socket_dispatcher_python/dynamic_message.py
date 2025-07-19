from dataclasses import dataclass
from typing import Any, Dict


@dataclass
class DynamicMessage:
    """Simple data holder for a message title and arbitrary payload."""

    title: str
    data: Dict[str, Any]
