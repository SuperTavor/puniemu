package models

type BadResponse struct {
	Result  bool   `json:"result"`
	Code    int    `json:"code"`
	Message string `json:"message"`
}

/*
Error codes:
4007 - Unknown APKey
4009 - Unknown UDKey
1002 - Internal Server Error
*/
func NewBadResponse(message string, code int) BadResponse {
	return BadResponse{
		//False to indicate bad response
		Result:  false,
		Code:    code,
		Message: message,
	}
}
