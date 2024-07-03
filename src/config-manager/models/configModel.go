package models

import (
	"encoding/json"
	"fmt"
	"os"
)

type ConfigModel struct {
	Paths               map[string]string `json:"paths"`
	StaticJsons         map[string]string `json:"static-jsons"`
	Port                string            `json:"port"`
	OgGameServerURL     string            `json:"ogGameServerUrl"`
	L5IDEmulatedAPIKey  string            `json:"l5idEmulatedApIKey"`
	MatchingGameVersion string            `json:"matchingGameVer"`
	MstVersionMaster    int               `json:"mstVersionMaster"`
	GetUpdateLink       string            `json:"getUpdateLink"`
}

func GetConfigModel() ConfigModel {
	const configPath string = "cfg.json"
	content, err := os.ReadFile(configPath)
	if err != nil {
		println("Could not read config file: " + err.Error())
	}
	var configModel ConfigModel
	err = json.Unmarshal(content, &configModel)
	if err != nil {
		println(fmt.Sprintf("Could not unmarshal config json: %s", err))
		os.Exit(1)
	}
	return configModel
}
