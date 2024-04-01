from __future__ import annotations
from kiota_abstractions.base_request_builder import BaseRequestBuilder
from kiota_abstractions.base_request_configuration import RequestConfiguration
from kiota_abstractions.get_path_parameters import get_path_parameters
from kiota_abstractions.method import Method
from kiota_abstractions.request_adapter import RequestAdapter
from kiota_abstractions.request_information import RequestInformation
from kiota_abstractions.request_option import RequestOption
from kiota_abstractions.serialization import Parsable, ParsableFactory
from typing import Any, Callable, Dict, List, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .....models.model_artifact_list import ModelArtifactList
    from .item.with_modelartifact_item_request_builder import WithModelartifactItemRequestBuilder

class Model_artifactsRequestBuilder(BaseRequestBuilder):
    """
    Path used to manage the list of modelartifacts.
    """
    def __init__(self,request_adapter: RequestAdapter, path_parameters: Union[str, Dict[str, Any]]) -> None:
        """
        Instantiates a new Model_artifactsRequestBuilder and sets the default values.
        param path_parameters: The raw url or the url-template parameters for the request.
        param request_adapter: The request adapter to use to execute the requests.
        Returns: None
        """
        super().__init__(request_adapter, "{+baseurl}/api/model_registry/v1alpha3/model_artifacts", path_parameters)
    
    def by_modelartifact_id(self,modelartifact_id: str) -> WithModelartifactItemRequestBuilder:
        """
        Path used to manage a single ModelArtifact.
        param modelartifact_id: A unique identifier for a `ModelArtifact`.
        Returns: WithModelartifactItemRequestBuilder
        """
        if not modelartifact_id:
            raise TypeError("modelartifact_id cannot be null.")
        from .item.with_modelartifact_item_request_builder import WithModelartifactItemRequestBuilder

        url_tpl_params = get_path_parameters(self.path_parameters)
        url_tpl_params["modelartifactId"] = modelartifact_id
        return WithModelartifactItemRequestBuilder(self.request_adapter, url_tpl_params)
    
    async def get(self,request_configuration: Optional[RequestConfiguration] = None) -> Optional[ModelArtifactList]:
        """
        Gets a list of all `ModelArtifact` entities.
        param request_configuration: Configuration for the request such as headers, query parameters, and middleware options.
        Returns: Optional[ModelArtifactList]
        """
        request_info = self.to_get_request_information(
            request_configuration
        )
        if not self.request_adapter:
            raise Exception("Http core is null") 
        from .....models.model_artifact_list import ModelArtifactList

        return await self.request_adapter.send_async(request_info, ModelArtifactList, None)
    
    def to_get_request_information(self,request_configuration: Optional[RequestConfiguration] = None) -> RequestInformation:
        """
        Gets a list of all `ModelArtifact` entities.
        param request_configuration: Configuration for the request such as headers, query parameters, and middleware options.
        Returns: RequestInformation
        """
        request_info = RequestInformation(Method.GET, self.url_template, self.path_parameters)
        request_info.configure(request_configuration)
        request_info.headers.try_add("Accept", "application/json")
        return request_info
    
    def with_url(self,raw_url: Optional[str] = None) -> Model_artifactsRequestBuilder:
        """
        Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        param raw_url: The raw URL to use for the request builder.
        Returns: Model_artifactsRequestBuilder
        """
        if not raw_url:
            raise TypeError("raw_url cannot be null.")
        return Model_artifactsRequestBuilder(self.request_adapter, raw_url)
    

