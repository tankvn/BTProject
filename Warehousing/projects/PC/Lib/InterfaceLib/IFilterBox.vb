Imports System.Windows.Forms
Imports UtilityLib.Controls

Public Interface IFilterBox
    Inherits IFilterPart

    Property ConditionComboBox() As ComboBox

    Property ColumnComboBox() As ComboBox

    Property FilterToggleButton() As FilterToggleButton
End Interface
