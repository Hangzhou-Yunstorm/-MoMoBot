$startTime = $(Get-Date)

docker-compose build

$elapsedTime = $(Get-Date) - $startTime

$elapsedTime

[console]::beep(900,400) 
[console]::beep(1000,400) 
[console]::beep(800,400) 
[console]::beep(400,400) 
[console]::beep(600,1600)