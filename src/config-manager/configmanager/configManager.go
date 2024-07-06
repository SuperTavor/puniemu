package configmanager

import (
	"os"

	"github.com/SuperTavor/Puniemu/src/config-manager/models"
)

var (
	CurrentConfig = models.GetConfigModel()
	StaticJsons   map[string]string
)

func LoadStaticJsons() {
	StaticJsons = make(map[string]string)
	for alias, path := range CurrentConfig.StaticJsons {
		content, err := os.ReadFile(path)
		if err != nil {
			println(err)
			return
		}

		StaticJsons[alias] = string(content)
	}
}
