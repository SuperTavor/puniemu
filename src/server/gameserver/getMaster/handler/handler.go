package handler

import (
	"net/http"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
)

func Handle(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	encodedMasterData, err := nhnrequests.EncryptAndEncode(configmanager.StaticJsons["MasterData"])
	if err != nil {
		println(err)
		return
	}
	w.Write([]byte(encodedMasterData))
}
