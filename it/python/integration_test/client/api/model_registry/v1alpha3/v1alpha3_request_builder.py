from __future__ import annotations
from kiota_abstractions.base_request_builder import BaseRequestBuilder
from kiota_abstractions.get_path_parameters import get_path_parameters
from kiota_abstractions.request_adapter import RequestAdapter
from typing import Any, Callable, Dict, List, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .model_artifacts.model_artifacts_request_builder import Model_artifactsRequestBuilder

class V1alpha3RequestBuilder(BaseRequestBuilder):
    """
    Builds and executes requests for operations under /api/model_registry/v1alpha3
    """
    def __init__(self,request_adapter: RequestAdapter, path_parameters: Union[str, Dict[str, Any]]) -> None:
        """
        Instantiates a new V1alpha3RequestBuilder and sets the default values.
        param path_parameters: The raw url or the url-template parameters for the request.
        param request_adapter: The request adapter to use to execute the requests.
        Returns: None
        """
        super().__init__(request_adapter, "{+baseurl}/api/model_registry/v1alpha3", path_parameters)
    
    @property
    def model_artifacts(self) -> Model_artifactsRequestBuilder:
        """
        Path used to manage the list of modelartifacts.
        """
        from .model_artifacts.model_artifacts_request_builder import Model_artifactsRequestBuilder

        return Model_artifactsRequestBuilder(self.request_adapter, self.path_parameters)
    

