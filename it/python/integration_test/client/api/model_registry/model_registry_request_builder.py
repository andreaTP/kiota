from __future__ import annotations
from kiota_abstractions.base_request_builder import BaseRequestBuilder
from kiota_abstractions.get_path_parameters import get_path_parameters
from kiota_abstractions.request_adapter import RequestAdapter
from typing import Any, Callable, Dict, List, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .v1alpha3.v1alpha3_request_builder import V1alpha3RequestBuilder

class Model_registryRequestBuilder(BaseRequestBuilder):
    """
    Builds and executes requests for operations under /api/model_registry
    """
    def __init__(self,request_adapter: RequestAdapter, path_parameters: Union[str, Dict[str, Any]]) -> None:
        """
        Instantiates a new Model_registryRequestBuilder and sets the default values.
        param path_parameters: The raw url or the url-template parameters for the request.
        param request_adapter: The request adapter to use to execute the requests.
        Returns: None
        """
        super().__init__(request_adapter, "{+baseurl}/api/model_registry", path_parameters)
    
    @property
    def v1alpha3(self) -> V1alpha3RequestBuilder:
        """
        The v1alpha3 property
        """
        from .v1alpha3.v1alpha3_request_builder import V1alpha3RequestBuilder

        return V1alpha3RequestBuilder(self.request_adapter, self.path_parameters)
    

