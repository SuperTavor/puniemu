package models

import (
	"time"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
)

type GetGdKeyAccountsResponse struct {
	//Timestamp when the response was sent.
	ServerDt int64 `json:"serverDt"`
	//Empty here.
	YWPToken string `json:"ywpToken"`
	//Version of assets on the server
	MstVersionVer int `json:"mstVersionVer"`
	//0 here
	ResultCode int `json:"resultCode"`
	//0 here
	NextScreenType  int               `json:"nextScreenType"`
	UDKeyPlayerList []UDKeyPlayerItem `json:"udkeyPlayerList"`
	//0 here
	ResultType int `json:"resultType"`
}

func NewGetGdKeyAccountsResponse(udkey string, gdkeys []string) (*GetGdKeyAccountsResponse, error) {
	var playerlist []UDKeyPlayerItem
	for _, gdkey := range gdkeys {
		playerItem, err := NewUDKeyPlayerItem(udkey, gdkey)
		if err != nil {
			return nil, err
		}
		playerlist = append(playerlist, *playerItem)
	}
	return &GetGdKeyAccountsResponse{
		ServerDt:        time.Now().Unix(),
		YWPToken:        "",
		UDKeyPlayerList: playerlist,
		MstVersionVer:   configmanager.CurrentConfig.MstVersionMaster,
		ResultCode:      0,
		NextScreenType:  0,
	}, nil
}
