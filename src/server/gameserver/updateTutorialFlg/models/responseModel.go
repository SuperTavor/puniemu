package models

import (
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
)

type UpdateTutorialFlagResponse struct {
	//Timestamp of when the response was sent.
	ServerDt int64 `json:"serverDt"`
	//Version of assets on the server
	MstVersionMaster int `json:"mstVersionMaster"`
	//0 here.
	ResultCode int `json:"resultCode"`
	//Table of completed tutorials for the user.
	TutorialList string `json:"ywp_user_tutorial_list"`
	//0 here.
	NextScreenType int `json:"nextScreenType"`
	//Basic userdata
	UserData gameServerModels.YwpUserData `json:"ywp_user_data"`
	//0 here.
	ResultType int `json:"resultType"`
}

func NewUpdateTutorialFlagResponse(tutorialList string, userdata gameServerModels.YwpUserData) *UpdateTutorialFlagResponse {
	return &UpdateTutorialFlagResponse{
		ServerDt:         time.Now().Unix(),
		MstVersionMaster: configmanager.CurrentConfig.MstVersionMaster,
		ResultCode:       0,
		TutorialList:     tutorialList,
		NextScreenType:   0,
		UserData:         userdata,
		ResultType:       0,
	}
}
