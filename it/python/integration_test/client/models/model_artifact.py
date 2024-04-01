from __future__ import annotations
from dataclasses import dataclass, field
from kiota_abstractions.serialization import Parsable, ParseNode, SerializationWriter
from typing import Any, Callable, Dict, List, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .with_base_artifact_update import WithBaseArtifactUpdate

from .with_base_artifact_update import WithBaseArtifactUpdate

@dataclass
class ModelArtifact(WithBaseArtifactUpdate):
    """
    An ML model artifact.
    """
    # The artifactType property
    artifact_type: Optional[str] = "model-artifact"
    
    @staticmethod
    def create_from_discriminator_value(parse_node: Optional[ParseNode] = None) -> ModelArtifact:
        """
        Creates a new instance of the appropriate class based on discriminator value
        param parse_node: The parse node to use to read the discriminator value and create the object
        Returns: ModelArtifact
        """
        if not parse_node:
            raise TypeError("parse_node cannot be null.")
        return ModelArtifact()
    
    def get_field_deserializers(self,) -> Dict[str, Callable[[ParseNode], None]]:
        """
        The deserialization information for the current model
        Returns: Dict[str, Callable[[ParseNode], None]]
        """
        from .with_base_artifact_update import WithBaseArtifactUpdate

        from .with_base_artifact_update import WithBaseArtifactUpdate

        fields: Dict[str, Callable[[Any], None]] = {
            "artifactType": lambda n : setattr(self, 'artifact_type', n.get_str_value()),
        }
        super_fields = super().get_field_deserializers()
        fields.update(super_fields)
        return fields
    
    def serialize(self,writer: SerializationWriter) -> None:
        """
        Serializes information the current object
        param writer: Serialization writer to use to serialize this model
        Returns: None
        """
        if not writer:
            raise TypeError("writer cannot be null.")
        super().serialize(writer)
        writer.write_str_value("artifactType", self.artifact_type)
    

