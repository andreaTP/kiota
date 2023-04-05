import apisdk.ApiClient;
import com.microsoft.kiota.authentication.AnonymousAuthenticationProvider;
import com.microsoft.kiota.http.OkHttpRequestAdapter;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;

public class BasicAPITest {

    @Test
    void basicTest() throws Exception {
        var adapter = new OkHttpRequestAdapter(new AnonymousAuthenticationProvider());
        var client = new ApiClient(adapter);

        client.foo("x").bar("y").get();
        client.baz("x"); // compiles
        // client.baz("x").and("y").get; // should be
    }

}
