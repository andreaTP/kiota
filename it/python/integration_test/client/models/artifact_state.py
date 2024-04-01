from enum import Enum

class ArtifactState(str, Enum):
    UNKNOWN = "UNKNOWN",
    PENDING = "PENDING",
    LIVE = "LIVE",
    MARKED_FOR_DELETION = "MARKED_FOR_DELETION",
    DELETED = "DELETED",
    ABANDONED = "ABANDONED",
    REFERENCE = "REFERENCE",

