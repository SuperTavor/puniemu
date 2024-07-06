package punitableparser

import (
	"errors"
	"strings"
)

type Table struct {
	Rows []Row
}

type Row struct {
	Items []string
}

// ItemIndices basically means that if the items i wanna search for are "3" and "ha" - at which index in the row should the function be looking for them?
func (tbl *Table) GetIndexByItems(itemIndices []int, itemsToSearch []string) (int, error) {
	if len(itemIndices) != len(itemsToSearch) {
		return -1, errors.New("ItemIndices and ItemToSearch should have the same length")
	}
	var outIndex int = -1
	for i, row := range tbl.Rows {
		var foundItems = 0
		for j, item := range itemsToSearch {
			if row.Items[itemIndices[j]] == item {
				foundItems++
			}
		}
		if foundItems == len(itemsToSearch) {
			outIndex = i
		}
	}
	return outIndex, nil
}
func (tbl *Table) AddRow(row Row) {
	tbl.Rows = append(tbl.Rows, row)
}
func (tbl *Table) String() string {
	var builder strings.Builder
	for i, row := range tbl.Rows {
		for j, item := range row.Items {
			builder.WriteString(item)
			//if not last item in the row, add pipe
			if j != len(row.Items)-1 {
				builder.WriteString("|")
			}
		}
		if i != len(tbl.Rows)-1 {
			builder.WriteString("*")
		}
	}
	return builder.String()
}
func NewTable(tableStr string) *Table {
	rowStrings := strings.Split(tableStr, "*")
	populatedRows := make([]Row, len(rowStrings))
	for i, rowStr := range rowStrings {
		populatedRows[i] = Row{Items: strings.Split(rowStr, "|")}
	}
	return &Table{Rows: populatedRows}
}
