package models

type GetGdKeyAccountsRequest struct {
	//Client version
	AppVer string `json:"appVer"`
	//UDKey
	DeviceID string `json:"deviceID"`
	/*List of gdkeys in this format:

	[
		{
			"gdkey" : "g-bvt9d702uuja2ipvlxepica4muohkxp2lf05hg9l8jnf0sg7fci2yrss7q2lho"
		}
	]

	*/
	GDKeys []map[string]string `json:"gdkeys"`
	//Always 0 here.
	Level5USerID string `json:"level5UserID"`
	//Version of assets on the server
	MstVersionVer int `json:"mstVersionVer"`
	//2 = Android. We are not focusing on supporting iOS right now.
	OsType int `json:"osType"`
	//0 here.
	UserID string `json:"userID"`
	//0 here.
	YWPToken string `json:"ywpToken"`
}
