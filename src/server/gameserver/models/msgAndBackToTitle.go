package models

import "github.com/SuperTavor/Puniemu/src/config-manager/configmanager"

type msgAndBackToTitle struct {
	//The dialog message to be displayed
	DialogMessage string `json:"dialogMsg"`
	//Can be gotten from the server's stored gameconsts.
	GameServerURL string `json:"gameServerUrl"`
	//always 1 when returning to title
	ResultCode int `json:"resultCode"`
	//should be an empty string to not send user to url
	StoreURL string `json:"storeUrl"`
	//the title of the dialog box
	DialogTitle string `json:"dialogTitle"`
	//always 1 when returning to title
	ResultType int `json:"resultType"`
	//Should be 3 when returning to title
	NextScreenType int `json:"nextScreenType"`
}

func NewMsgAndBackToTitle(dialogMsg, dialogTitle string) msgAndBackToTitle {
	return msgAndBackToTitle{
		DialogMessage:  dialogMsg,
		GameServerURL:  configmanager.CurrentConfig.OgGameServerURL,
		ResultCode:     1,
		StoreURL:       "",
		DialogTitle:    dialogTitle,
		ResultType:     1,
		NextScreenType: 3,
	}
}

/*
Example:
{
    "dialogMsg": "Unsupported server operation.\nPlease report this bug!",
    "gameServerUrl": "https:\/\/gameserver.yw-p.com",
    "resultCode": 1,
    "storeUrl": "",
    "dialogTitle": "",
    "resultType": 1,
    "nextScreenType": 3
}
*/
