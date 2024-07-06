package models

type UpdateTutorialFlagRequest struct {
	//udkey
	DeviceID string `json:"deviceId"`
	//gdkey
	Level5UserID string `json:"level5UserId"`
	//ID of the tutorial to be set.
	TutorialID int `json:"tutorialId"`
	//Tutorial status to give that tutorial ID
	TutorialStatus int `json:"tutorialStatus"`
	//Type of that tutorial.
	TutoriaType int `json:"tutorialType"`
}
