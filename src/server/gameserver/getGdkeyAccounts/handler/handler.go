package handler

import (
	"encoding/json"
	"io"
	"log"
	"net/http"

	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
	models "github.com/SuperTavor/Puniemu/src/server/gameserver/getGdkeyAccounts/models"
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
	var request models.GetGdKeyAccountsRequest
	err = json.Unmarshal([]byte(decryptedRequest), &request)
	if err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	//Convert gdkey map to gdkey list
	gdkeyList := make([]string, len(request.GDKeys))
	for i := 0; i < len(request.GDKeys); i++ {
		gdkeyList[i] = request.GDKeys[i]["gdkey"]
	}
	response, err := models.NewGetGdKeyAccountsResponse(request.DeviceID, gdkeyList)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	marshalledResponse, err := json.Marshal(response)
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	encryptedResponse, err := nhnrequests.EncryptAndEncode(string(marshalledResponse))
	if err != nil {
		log.Println(err)
		http.Error(w, "Internal server error", http.StatusInternalServerError)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(encryptedResponse))
}
