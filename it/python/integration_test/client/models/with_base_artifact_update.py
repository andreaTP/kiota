from __future__ import annotations
from dataclasses import dataclass, field
from kiota_abstractions.serialization import AdditionalDataHolder, Parsable, ParseNode, SerializationWriter
from typing import Any, Callable, Dict, List, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .artifact_state import ArtifactState

@dataclass
class WithBaseArtifactUpdate(AdditionalDataHolder, Parsable):
    # Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
    additional_data: Dict[str, Any] = field(default_factory=dict)

    from .artifact_state import ArtifactState

    #  - PENDING: A state indicating that the artifact may exist. - LIVE: A state indicating that the artifact should exist, unless somethingexternal to the system deletes it. - MARKED_FOR_DELETION: A state indicating that the artifact should be deleted. - DELETED: A state indicating that the artifact has been deleted. - ABANDONED: A state indicating that the artifact has been abandoned, which may bedue to a failed or cancelled execution. - REFERENCE: A state indicating that the artifact is a reference artifact. Atexecution start time, the orchestrator produces an output artifact foreach output key with state PENDING. However, for an intermediateartifact, this first artifact's state will be REFERENCE. Intermediateartifacts emitted during a component's execution will copy the REFERENCEartifact's attributes. At the end of an execution, the artifact stateshould remain REFERENCE instead of being changed to LIVE.See also: ml-metadata Artifact.State
    state: Optional[ArtifactState] = ArtifactState("UNKNOWN")
    # The uniform resource identifier of the physical artifact.May be empty if there is no physical artifact.
    uri: Optional[str] = None
    
    @staticmethod
    def create_from_discriminator_value(parse_node: Optional[ParseNode] = None) -> WithBaseArtifactUpdate:
        """
        Creates a new instance of the appropriate class based on discriminator value
        param parse_node: The parse node to use to read the discriminator value and create the object
        Returns: WithBaseArtifactUpdate
        """
        if not parse_node:
            raise TypeError("parse_node cannot be null.")
        return WithBaseArtifactUpdate()
    
    def get_field_deserializers(self,) -> Dict[str, Callable[[ParseNode], None]]:
        """
        The deserialization information for the current model
        Returns: Dict[str, Callable[[ParseNode], None]]
        """
        from .artifact_state import ArtifactState

        from .artifact_state import ArtifactState

        fields: Dict[str, Callable[[Any], None]] = {
            "state": lambda n : setattr(self, 'state', n.get_enum_value(ArtifactState)),
            "uri": lambda n : setattr(self, 'uri', n.get_str_value()),
        }
        return fields
    
    def serialize(self,writer: SerializationWriter) -> None:
        """
        Serializes information the current object
        param writer: Serialization writer to use to serialize this model
        Returns: None
        """
        if not writer:
            raise TypeError("writer cannot be null.")
        writer.write_enum_value("state", self.state)
        writer.write_str_value("uri", self.uri)
        writer.write_additional_data_value(self.additional_data)
    

