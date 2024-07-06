package models

import (
	"encoding/base32"
	"encoding/binary"
	"hash/crc32"
	"strconv"
	"time"
)

type YwpUserData struct {
	// User's birthday. Empty unless set.
	Birthday string `json:"birthday"`
	// Free energy right now. Usually 5 at the start of the game.
	FreeHitodama int `json:"freeHitodama"`
	//Have no idea. Initialized to 0.
	FriendMaxCount int `json:"friendMaxCnt"`
	// Have no idea. Initialized to 0.
	MedalPoint int `json:"medalPoint"`
	// ID of the current stage the user is at. Defaults to 1001001 (first lvl)
	CurrentStageID int `json:"nowStageId"`
	// ID of the character's title (e.g., kun, chan).
	CharacterTitleID int `json:"titleId"`
	// Have no idea. Initialized to 0.
	GokuCollectCount int `json:"gokuCollectCnt"`
	// Amount of YMoney. Initialized at 3000
	YMoney int `json:"ymoney"`
	// Have no idea. Initialized to 0.
	ReviewFlag int `json:"reviewFlg"`
	// Have no idea. Initialized to 0.
	MoveReason int `json:"moveReason"`
	// Have no idea. Initialized to 0.
	ChargeYmoney int `json:"chargeYmoney"`
	//Have no idea. Initialized to 0.
	CrystalCollectCount int `json:"crystalCollectCnt"`
	// Have no idea. Initialized to 0.
	EventPointUpItemID int `json:"eventPointUpItemId"`
	// Friend code of the current user. Example: "8wea1pxb".
	CharacterID string `json:"characterId"`
	// ID of the user's icon. Initialized to 0.
	IconID int `json:"iconId"`
	// Have no idea. Initialized to 0.
	LimitTimeSaleRemainSec int `json:"limitTimeSaleRemainSec"`
	// Have no idea. Initialized to 0.
	TotMedalPoint int `json:"totMedalPoint"`
	// Have no idea. Initialized to 0.
	EventPointUpItemRemainSec int `json:"eventPointUpItemRemainSec"`
	// Player's name.
	PlayerName string `json:"playerName"`
	// Remaining seconds for today. Initialized to 0.
	TodaysRemainSec int `json:"todaysRemainSec"`
	// Weekly free flag. Initialized to 0.
	WeeklyFreeFlag int `json:"weeklyFreeFlg"`
	// Energy count. Initialized to 0.
	Hitodama int `json:"hitodama"`
	// User ID. 0 here, as this specific user ID is not yet used.
	UserID string `json:"userId"`
	// List of items in use.
	UsingItemList []int `json:"usingItemList"`
	// Seconds left until energy recovers
	HitodamaRecoverSec int `json:"hitodamaRecoverSec"`
	// End date of the limit time sale. Empty unless set.
	LimitTimeSaleEndDt string `json:"limitTimeSaleEndDt"`
	// ID of the equipped watch. Initialized to 0.
	EquipWatchID int `json:"equipWatchId"`
}

const (
	ICON_BOY  = 1
	ICON_GIRL = 2
)
const (
	TITLE_KUN  = 1
	TITLE_CHAN = 2
)

func NewUserData(iconID int, gdkey, playerName string) *YwpUserData {
	//First IconID can only be either 1 (male) or 2 (female). This is how we calculate the player's title.
	playerTitle := iconID
	return &YwpUserData{
		Birthday:                  "",
		FreeHitodama:              5,
		FriendMaxCount:            0,
		MedalPoint:                0,
		CurrentStageID:            1001001,
		CharacterTitleID:          playerTitle,
		GokuCollectCount:          0,
		YMoney:                    3000,
		ReviewFlag:                0,
		MoveReason:                0,
		ChargeYmoney:              0,
		CrystalCollectCount:       0,
		EventPointUpItemID:        0,
		CharacterID:               generateFriendCode(gdkey),
		IconID:                    iconID,
		LimitTimeSaleRemainSec:    0,
		TotMedalPoint:             0,
		EventPointUpItemRemainSec: 0,
		PlayerName:                playerName,
		TodaysRemainSec:           getRemainSec(),
		WeeklyFreeFlag:            0,
		Hitodama:                  0,
		UserID:                    "0",
		UsingItemList:             make([]int, 0),
		HitodamaRecoverSec:        0,
		LimitTimeSaleEndDt:        "",
		EquipWatchID:              0,
	}
}
func getRemainSec() int {
	now := time.Now()
	secondsPassed := now.Hour()*3600 + now.Minute()*60 + now.Second()
	const SECONDS_IN_DAY = 86400
	return SECONDS_IN_DAY - secondsPassed
}
func generateFriendCode(gdkey string) string {
	timestamp := []byte(strconv.FormatInt(time.Now().Unix(), 10))
	friendCodeInt := crc32.ChecksumIEEE(append([]byte(gdkey), timestamp...))
	var friendCodeBytes = make([]byte, 4)
	binary.LittleEndian.PutUint32(friendCodeBytes, friendCodeInt)
	encoded := base32.StdEncoding.EncodeToString(friendCodeBytes)
	return encoded
}
