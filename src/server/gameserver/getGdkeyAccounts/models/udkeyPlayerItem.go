package models

import (
	"encoding/binary"
	"encoding/json"

	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
	userdatamanager "github.com/SuperTavor/Puniemu/src/userDataManager"
)

type UDKeyPlayerItem struct {
	//ID of the player's icon.
	IconID int `json:"iconID"`
	//Name of the player
	PlayerName string `json:"playerName"`
	//The center yokai in the party, which is displayed next to the user's name.
	//Since we don't have this implemented yet, it will default to 2235000.
	PartnerYokaiID int `json:"youkaiId"`
	//Last time the game was played. Since we don't have access to this info just yet, we'll leave this as 1970-01-01 00:00:00
	LastUpdateDate string `json:"lastUpdateDate"`
	//Title of the character (kun, chan, etc...)
	TitleID int    `json:"titleId"`
	GDKey   string `json:"gdkey"`
	//in Puniemu, the UserID is unused - only the udkey and gdkey are used. This will be left as 0.
	UserID string `json:"userId"`
	//First time the game was played.
	StartDate string `json:"playStartDate"`
}

func NewUDKeyPlayerItem(udkey, gdkey string) (*UDKeyPlayerItem, error) {
	var userData gameServerModels.YwpUserData
	userDataJson, err := userdatamanager.GetYwpUser(udkey, gdkey, "ywp_user_data")
	if err != nil {
		return nil, err
	}
	err = json.Unmarshal(userDataJson, &userData)
	if err != nil {
		return nil, err
	}
	playerItem := UDKeyPlayerItem{}

	playerItem.IconID = userData.IconID
	playerItem.PlayerName = userData.PlayerName
	//2235000 until we have access to this property
	playerItem.PartnerYokaiID = 2235000
	startTimestamp, err := userdatamanager.GetYwpUser(udkey, gdkey, "START_DATE")
	if err != nil {
		return nil, err
	}
	startTimeString := userdatamanager.UnixTimeToDate(int64(binary.LittleEndian.Uint64(startTimestamp)))
	playerItem.StartDate = startTimeString
	//Placeholder - this will be changed when implementing login.nhn
	playerItem.LastUpdateDate = "1970-01-01 00:00:00"
	playerItem.TitleID = userData.CharacterTitleID
	playerItem.GDKey = gdkey
	playerItem.UserID = "0"

	return &playerItem, nil
}
