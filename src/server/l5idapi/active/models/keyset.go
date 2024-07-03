package models

import l5idapi_models "github.com/SuperTavor/Puniemu/src/server/l5idapi/models"

type KeySet struct {
	//ID for the player's client.
	UDKey l5idapi_models.Key `json:"udkey"`
	//GDKeys are IDs of the saves on the server. They can be retrieved with the UDKey.
	GDKeys []l5idapi_models.Key `json:"gdkeys"`
}
