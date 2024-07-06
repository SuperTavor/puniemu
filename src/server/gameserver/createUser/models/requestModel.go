package models

type CreateUserRequest struct {
	//Client version
	AppVer string `json:"appVer"`
	//Equivalant to UDKey.
	DeviceID string `json:"deviceID"`
	//ID of the user icon.
	IconID int `json:"iconID"`
	//Interchangable with gdkey
	Level5UserID string `json:"level5UserID"`
	//Version of server data
	MstVersionVer int `json:"mstVersionVer"`
	//OsType. 2 is Android. We typically ignore this field, though, because iOS is not in our priorities right now.
	OsType int `json:"osType"`
	//Player's name
	PlayerName string `json:"playerName"`
	//Seems to be `\u0001` no matter which gender you choose.
	PlayerSex string `json:"playerSexType"`
	//Always 0 here.
	UserID string `json:"userID"`
	//Always 0.
	YwpToken string `json:"ywpToken"`
}
