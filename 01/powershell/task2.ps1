(Get-Clipboard | % { $x = [int]$_; Get-Clipboard | % { $y = [int]$_; Get-Clipboard | % { $z = [int]$_; if ($x + $y + $z -eq 2020) { return $x * $y * $z; break; } } } })[0] | clip