param([string]$BaseUrl = 'http://127.0.0.1:5187')
$ErrorActionPreference = 'Stop'
$tag = 'Smoke' + [Guid]::NewGuid().ToString('N').Substring(0, 12)
$email = "$tag@example.test"
$guest = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$member = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$facilityId = 0
$typeId = 0
$passed = 0

function Sql([string]$query) {
    $result = & sqlcmd -S 127.0.0.1 -d SportsBooking -E -C -b -h -1 -W -Q "SET NOCOUNT ON; $query"
    if ($LASTEXITCODE -ne 0) { throw "SQL failed: $result" }
    return ($result -join '').Trim()
}
function GetPage($path, $session) {
    return Invoke-WebRequest "$BaseUrl$path" -WebSession $session
}
function PostForm($path, $data, $session) {
    $page = GetPage $path $session
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value
    if (!$token) { throw "Missing form token on $path" }
    $data['__RequestVerificationToken'] = [Net.WebUtility]::HtmlDecode($token)
    return Invoke-WebRequest "$BaseUrl$path" -Method Post -Body $data -WebSession $session
}
function Check($condition, [string]$name) {
    if (!$condition) { throw "FAIL: $name" }
    $script:passed++
    Write-Output "PASS: $name"
}

try {
    $typeId = [int](Sql "INSERT INTO FacilityType (TypeName) VALUES ('$tag'); SELECT SCOPE_IDENTITY();")
    $facilityId = [int](Sql "INSERT INTO Facility (Name,TypeID,Location,Capacity,HourlyRate) VALUES ('$tag',$typeId,'Smoke Test Area',4,100); SELECT SCOPE_IDENTITY();")
    Check ((GetPage '/' $guest).Content.Contains('Community Sports Facilities')) 'Home page'
    $page = GetPage "/Facility/Search?TypeId=$typeId" $guest
    Check ($page.Content.Contains($tag) -and !$page.Content.Contains('Hourly rate')) 'Guest facility search hides booking details'
    foreach ($path in @('/Booking/Create','/Booking/MyBookings','/Review/Create')) {
        $page = GetPage $path $guest
        Check ($page.BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Account/Login') "Guest blocked from $path"
    }
    foreach ($path in @('/Member','/Facility/Create','/Facility/Edit/1','/Review/Delete/1','/Inquiry','/SportPreference','/FacilityType')) {
        $response = Invoke-WebRequest "$BaseUrl$path" -WebSession $guest -SkipHttpErrorCheck
        Check ($response.StatusCode -eq 404) "Unused management route disabled: $path"
    }
    $page = PostForm '/Account/Login' @{email=$email;password='wrong'} $guest
    Check ($page.Content.Contains('Invalid email or password')) 'Wrong login rejected'
    $page = PostForm '/Account/Register' @{Name=$tag;Email=$email;Phone='0770000000';Address='Test';Password='Test123!';SelectedSports='999999'} $guest
    Check ($page.Content.Contains('Choose a sport from the list')) 'Invalid sport rejected'
    $page = PostForm '/Account/Register' @{Name=$tag;Email=$email;Phone='0770000000';Address='Test';Password='Test123!';SelectedSports="$typeId"} $member
    Check ($page.BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Facility/Search') 'Registration and automatic login'
    $memberId = [int](Sql "SELECT MemberID FROM Member WHERE Email='$email';")
    Check ((Sql "SELECT COUNT(*) FROM SportPreference WHERE MemberID=$memberId AND TypeID=$typeId;") -eq '1') 'Sports preference saved'
    $page = PostForm '/Account/Register' @{Name=$tag;Email=$email;Phone='0770000000';Address='Test';Password='Test123!';SelectedSports="$typeId"} $guest
    Check ($page.Content.Contains('already registered')) 'Duplicate email rejected'
    $page = PostForm '/Account/Login' @{email=$email;password='Test123!'} $guest
    Check ($page.BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Facility/Search') 'Valid login'
    $page = GetPage "/Facility/Search?TypeId=$typeId" $member
    Check ($page.Content.Contains('Hourly rate') -and $page.Content.Contains("/Booking/Create?facilityId=$facilityId")) 'Member search shows booking link'
    $date = (Get-Date).AddDays(30).ToString('yyyy-MM-dd')
    $page = PostForm '/Booking/Create' @{FacilityId="$facilityId";BookingDate=$date;StartTime='10:00';EndTime='11:00';MemberId='999999';Status='Confirmed'} $member
    Check ($page.Content.Contains('Your booking request has been saved')) 'Booking saved'
    Check ((Sql "SELECT COUNT(*) FROM Booking WHERE FacilityID=$facilityId AND MemberID=$memberId AND Status='Pending';") -eq '1') 'Member and status cannot be forged'
    $page = PostForm '/Booking/Create' @{FacilityId="$facilityId";BookingDate=$date;StartTime='10:30';EndTime='11:30'} $member
    Check ($page.Content.Contains('already booked')) 'Overlapping booking rejected'
    $page = GetPage "/Facility/Search?TypeId=$typeId&Date=$date&StartTime=10:30&EndTime=11:30" $member
    Check ($page.Content.Contains('No facilities match')) 'Booked facility excluded from search'
    $page = PostForm '/Booking/Create' @{FacilityId="$facilityId";BookingDate=$date;StartTime='11:00';EndTime='12:00'} $member
    Check ($page.Content.Contains('Your booking request has been saved')) 'Adjacent booking allowed'
    $page = PostForm '/Booking/Create' @{FacilityId="$facilityId";BookingDate=$date;StartTime='12:00';EndTime='11:00'} $member
    Check ($page.Content.Contains('End time must be after start time')) 'Invalid time rejected'
    $page = PostForm '/Booking/Create' @{FacilityId="$facilityId";BookingDate='2020-01-01';StartTime='10:00';EndTime='11:00'} $member
    Check ($page.Content.Contains('Choose a future date and time')) 'Past booking rejected'
    $page = PostForm '/Review/Create' @{FacilityId="$facilityId";Rating='4';Comments=$tag} $member
    Check ($page.Content.Contains('after a confirmed booking has ended')) 'Review before use rejected'
    Sql "INSERT INTO Booking (MemberID,FacilityID,BookingDate,StartTime,EndTime,Status) VALUES ($memberId,$facilityId,'2020-01-01','2020-01-01T10:00:00','2020-01-01T11:00:00','Confirmed');" | Out-Null
    $page = PostForm '/Review/Create' @{FacilityId="$facilityId";Rating='6';Comments=$tag} $member
    Check ($page.Content.Contains('must be between 1 and 5')) 'Invalid rating rejected'
    $page = PostForm '/Review/Create' @{FacilityId="$facilityId";Rating='4';Comments=$tag} $member
    Check ($page.Content.Contains('Your review has been saved')) 'Review saved'
    $page = GetPage "/Review?facilityId=$facilityId&rating=4" $guest
    Check ($page.Content.Contains($tag)) 'Review search shows facility and comments'
    $page = PostForm '/Inquiry/Create' @{GuestName=$tag;Email=$email;Message=''} $guest
    Check ($page.Content.Contains('Message field is required')) 'Empty inquiry rejected'
    $page = PostForm '/Inquiry/Create' @{GuestName=$tag;Email=$email;Message=$tag} $guest
    Check ($page.Content.Contains('Your inquiry has been sent')) 'Inquiry confirmation'
    Check ((Sql "SELECT COUNT(*) FROM Inquiry WHERE Email='$email' AND Message='$tag' AND DateSent=CAST(GETDATE() AS date);") -eq '1') 'Inquiry saved with server date'
    $page = GetPage '/' $member
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value
    $page = Invoke-WebRequest "$BaseUrl/Account/Logout" -Method Post -Body @{__RequestVerificationToken=$token} -WebSession $member
    Check ((GetPage '/Booking/MyBookings' $member).BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Account/Login') 'Logout clears session'
    Write-Output "$passed checks passed."
}
finally {
    Sql "DELETE FROM Review WHERE FacilityID=$facilityId; DELETE FROM Booking WHERE FacilityID=$facilityId; DELETE FROM SportPreference WHERE MemberID IN (SELECT MemberID FROM Member WHERE Email='$email'); DELETE FROM Inquiry WHERE Email='$email'; DELETE FROM Member WHERE Email='$email'; DELETE FROM Facility WHERE FacilityID=$facilityId; DELETE FROM FacilityType WHERE TypeID=$typeId;" | Out-Null
    Write-Output 'Temporary test records removed.'
}
