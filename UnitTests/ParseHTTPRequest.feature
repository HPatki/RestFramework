Feature: ParseHTTPRequest
	Having received an HTTP request, the framework
	should parse the request without any errors
	return an appropriate message

@mytag
Scenario: GET Http Message with empty Body
	Given receiving an HTTP GET Message with Empty Body
	Then return HTTP status 200
