package models

type RCClientVersion struct {
	One string `json:"1"`
	Two string `json:"2"`
}

// Both variables are always empty in the real responses
func NewRCClientVersion() RCClientVersion {
	return RCClientVersion{One: "", Two: ""}
}
