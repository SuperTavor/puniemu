package main

import (
	"encoding/json"
	"fmt"
	"net/http"
	"os"

	"github.com/SuperTavor/Puniemu/src/config-manager/configmanager"
	nhnrequests "github.com/SuperTavor/Puniemu/src/nhnRequests"
	createUserHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/createUser/handler"
	gdkeyAccsHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/getGdkeyAccounts/handler"
	l5idStatusHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/getL5idStatus/handler"
	getMasterHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/getMaster/handler"
	initHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/init/handler"
	generalGameServerModels "github.com/SuperTavor/Puniemu/src/server/gameserver/models"
	updateTutorialFlgHandler "github.com/SuperTavor/Puniemu/src/server/gameserver/updateTutorialFlg/handler"
	imgserver "github.com/SuperTavor/Puniemu/src/server/imgServer"
	activeHandler "github.com/SuperTavor/Puniemu/src/server/l5idapi/active/handler"
	create_gdkeyHandler "github.com/SuperTavor/Puniemu/src/server/l5idapi/create_gdkey/handler"
	userdatamanager "github.com/SuperTavor/Puniemu/src/userDataManager"
)

func main() {
	loadServerFiles()
	setLevel5APIHandles()
	setGameserverHandles()
	setImgServerHandles()
	//Add the unsupported dialog for all unsupported requests
	http.HandleFunc("/", defaultHandler)
	go func() {
		port := configmanager.CurrentConfig.Port
		fmt.Printf("Starting server on port %s\n", port)
		err := http.ListenAndServe(fmt.Sprintf(":%s", port), nil)
		if err != nil {
			fmt.Println("Error starting server:", err)
		}
	}()
	waitForKeyPress()
}
func defaultHandler(w http.ResponseWriter, r *http.Request) {
	msg := generalGameServerModels.NewMsgAndBackToTitle(fmt.Sprintf("Unsupported server operation:\n%s", r.URL.Path), "Puniemu")
	msgJson, _ := json.Marshal(msg)
	result, err := nhnrequests.EncryptAndEncode(string(msgJson[:]))
	if err != nil {
		println(err)
		return
	}
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(result))
}
func setLevel5APIHandles() {
	http.HandleFunc("/l5id/api/v1/active/", activeHandler.Handle)
	http.HandleFunc("/l5id/api/v1/create_gdkey", create_gdkeyHandler.Handle)

}
func setImgServerHandles() {
	http.HandleFunc("/dataDownload/", imgserver.Handle)
}
func setGameserverHandles() {
	http.HandleFunc("/getMaster.nhn", getMasterHandler.Handle)
	http.HandleFunc("/init.nhn", initHandler.Handle)
	http.HandleFunc("/createUser.nhn", createUserHandler.Handle)
	http.HandleFunc("/getGdkeyAccounts.nhn", gdkeyAccsHandler.Handle)
	http.HandleFunc("/getL5IdStatus.nhn", l5idStatusHandler.Handle)
	http.HandleFunc("/updateTutorialFlg.nhn", updateTutorialFlgHandler.Handle)
}

func loadServerFiles() {
	configmanager.LoadStaticJsons()
	userdatamanager.InitializeDB()
}

func waitForKeyPress() {
	println("Press any key to exit")
	var input []byte = make([]byte, 1)
	_, _ = os.Stdin.Read(input)
	fmt.Println("Exiting...")
	gracefulShutdown()
}

func gracefulShutdown() {
	userdatamanager.CloseDB()
	os.Exit(0)
}
