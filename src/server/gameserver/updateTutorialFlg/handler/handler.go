package handler

import (
	"encoding/json"
	"io"
	"net/http"
	"strconv"

	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
	punitableparser "github.com/SuperTavor/Puniemu/src/puniTableParser"
	gameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
	models "github.com/SuperTavor/Puniemu/src/server/gameserver/updateTutorialFlg/models"
	userdatamanager "github.com/SuperTavor/Puniemu/src/userDataManager"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	encryptedRequest, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "No request.", http.StatusBadRequest)
		return
	}
	decryptedRequest, err := nhnrequests.DecodeAndDecrypt(string(encryptedRequest))
	if err != nil {
		http.Error(w, "Bad request encryption/encoding", http.StatusBadRequest)
		return
	}
	var request models.UpdateTutorialFlagRequest
	err = json.Unmarshal([]byte(decryptedRequest), &request)
	if err != nil {
		http.Error(w, "Bad request formatting", http.StatusBadRequest)
		return
	}

	tutorialList, err := userdatamanager.GetYwpUser(request.DeviceID, request.Level5UserID, "ywp_user_tutorial_list")
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	tutorialListTable := punitableparser.NewTable(string(tutorialList))
	//Format of tutorial list tutorialRow: TutorialType, TutorialID, TutorialStatus
	//Create tutorialRow with modified attributes
	tutorialRow := punitableparser.Row{Items: []string{strconv.Itoa(request.TutoriaType), strconv.Itoa(request.TutorialID), strconv.Itoa(request.TutorialStatus)}}
	//Search by tutorial ID
	index, err := tutorialListTable.GetIndexByItems([]int{1}, []string{strconv.Itoa(request.TutorialID)})
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	//if not found, we add. Else, we modify the found index
	if index == -1 {
		tutorialListTable.AddRow(tutorialRow)
	} else {
		tutorialListTable.Rows[index] = tutorialRow
	}
	updatedTutorialListStr := tutorialListTable.String()
	//Store modified tutorial list in database
	userdatamanager.StoreYwpUser(request.DeviceID, request.Level5UserID, "ywp_user_tutorial_list", []byte(updatedTutorialListStr))
	//begin constructing response
	userdataJson, err := userdatamanager.GetYwpUser(request.DeviceID, request.Level5UserID, "ywp_user_data")
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	var userdata gameServerModels.YwpUserData
	err = json.Unmarshal(userdataJson, &userdata)
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	res := models.NewUpdateTutorialFlagResponse(updatedTutorialListStr, userdata)
	marshalledRes, err := json.Marshal(res)
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	encryptedResponse, err := nhnrequests.EncryptAndEncode(string(marshalledRes))
	if err != nil {
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(encryptedResponse))
}
