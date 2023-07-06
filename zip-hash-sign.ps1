$appPath = (get-item $pwd).parent.parent.FullName
$fileName = "RandomThings.zip"
$fullName = $pwd.Path + "\..\" + $fileName
Compress-Archive -Path '*' -DestinationPath $fullName -Force
