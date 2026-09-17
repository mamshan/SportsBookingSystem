const fs = require('fs');
const path = require('path');
const {execFileSync} = require('child_process');
const {chromium} = require('C:/Users/Shan/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright');
const out = path.resolve(__dirname, 'evidence');
fs.mkdirSync(out, {recursive: true});
const sqlcmd = 'C:/Program Files/Microsoft SQL Server/Client SDK/ODBC/170/Tools/Binn/SQLCMD.EXE';
function sql(query) {
    return execFileSync(sqlcmd, ['-S','127.0.0.1','-d','SportsBooking','-E','-C','-b','-h','-1','-y','8000','-w','65535','-Q','SET NOCOUNT ON; '+query], {encoding:'utf8',windowsHide:true}).split(/\r?\n/).map(s=>s.trim()).join('');
}
function json(query) { return JSON.parse(sql(query + ' FOR JSON PATH') || '[]'); }
const queries = [
    ['Bookings with member and facility names', "SELECT TOP (10) b.BookingID,m.Name AS Member,f.Name AS Facility,b.BookingDate,b.Status FROM Booking b JOIN Member m ON b.MemberID=m.MemberID JOIN Facility f ON b.FacilityID=f.FacilityID ORDER BY b.BookingID;"],
    ['Facilities available for a selected time', "SELECT f.FacilityID,f.Name,f.Location FROM Facility f WHERE NOT EXISTS (SELECT 1 FROM Booking b WHERE b.FacilityID=f.FacilityID AND b.BookingDate='2026-10-17' AND b.Status<>'Cancelled' AND (b.StartTime IS NULL OR b.EndTime IS NULL OR (b.StartTime<'2026-10-17T11:00:00' AND b.EndTime>'2026-10-17T10:00:00'))) ORDER BY f.FacilityID;"],
    ['Average rating for each reviewed facility', "SELECT f.Name,COUNT(*) AS Reviews,CAST(AVG(CAST(r.Rating AS DECIMAL(5,2))) AS DECIMAL(5,2)) AS AverageRating FROM Review r JOIN Facility f ON r.FacilityID=f.FacilityID GROUP BY f.FacilityID,f.Name ORDER BY AverageRating DESC;"],
    ['Five least expensive facilities', "SELECT TOP (5) Name,Location,HourlyRate FROM Facility WHERE HourlyRate IS NOT NULL ORDER BY HourlyRate,FacilityID;"],
    ['Members who have made a booking', "SELECT MemberID,Name FROM Member WHERE MemberID IN (SELECT MemberID FROM Booking) ORDER BY MemberID;"],
    ['Facilities with no booking records', "SELECT f.FacilityID,f.Name FROM Facility f LEFT JOIN Booking b ON f.FacilityID=b.FacilityID WHERE b.BookingID IS NULL ORDER BY f.FacilityID;"],
    ['Preferred sports for each member', "SELECT m.Name AS Member,t.TypeName AS PreferredSport FROM SportPreference s JOIN Member m ON m.MemberID=s.MemberID JOIN FacilityType t ON t.TypeID=s.TypeID ORDER BY m.MemberID,t.TypeName;"],
    ['Latest guest inquiries', "SELECT TOP (5) InquiryID,GuestName,Message,DateSent FROM Inquiry ORDER BY DateSent DESC,InquiryID DESC;"]
];
const snapshot = {
    recordedAt: new Date().toISOString(),
    columns: json("SELECT t.name AS TableName,c.name AS ColumnName,ty.name AS DataType,c.max_length AS MaxLength,c.precision AS Precision,c.scale AS Scale,c.is_nullable AS Nullable,c.is_identity AS IsIdentity FROM sys.tables t JOIN sys.columns c ON t.object_id=c.object_id JOIN sys.types ty ON c.user_type_id=ty.user_type_id WHERE t.name IN ('Member','FacilityType','Facility','Booking','Review','SportPreference','Inquiry') ORDER BY t.name,c.column_id"),
    keys: json("SELECT t.name AS TableName,c.name AS ColumnName,k.name AS ConstraintName,k.type_desc AS Kind FROM sys.key_constraints k JOIN sys.tables t ON k.parent_object_id=t.object_id JOIN sys.index_columns ic ON ic.object_id=t.object_id AND ic.index_id=k.unique_index_id JOIN sys.columns c ON c.object_id=t.object_id AND c.column_id=ic.column_id ORDER BY t.name,k.name,ic.key_ordinal"),
    foreignKeys: json("SELECT OBJECT_NAME(f.parent_object_id) AS TableName,COL_NAME(fc.parent_object_id,fc.parent_column_id) AS ColumnName,OBJECT_NAME(f.referenced_object_id) AS TargetTable,COL_NAME(fc.referenced_object_id,fc.referenced_column_id) AS TargetColumn,f.name AS ConstraintName FROM sys.foreign_keys f JOIN sys.foreign_key_columns fc ON f.object_id=fc.constraint_object_id"),
    checks: json("SELECT OBJECT_NAME(parent_object_id) AS TableName,name AS ConstraintName,definition AS Definition FROM sys.check_constraints"),
    counts: json("SELECT 'Member' AS TableName,COUNT(*) AS Rows FROM Member UNION ALL SELECT 'FacilityType',COUNT(*) FROM FacilityType UNION ALL SELECT 'Facility',COUNT(*) FROM Facility UNION ALL SELECT 'Booking',COUNT(*) FROM Booking UNION ALL SELECT 'Review',COUNT(*) FROM Review UNION ALL SELECT 'SportPreference',COUNT(*) FROM SportPreference UNION ALL SELECT 'Inquiry',COUNT(*) FROM Inquiry"),
    queries: queries.map(([title,query]) => ({title,sql:query,rows:json(query.replace(/;$/,''))}))
};
fs.writeFileSync(path.join(out,'database.json'), JSON.stringify(snapshot,null,2));
fs.writeFileSync(path.join(__dirname,'queries.sql'), queries.map(([title,query],i)=>'-- Query '+(i+1)+': '+title+'\n'+query+'\nGO\n').join('\n'));
const tag = 'Report'+Date.now();
const email = tag+'@example.test';
let typeId=0,facilityId=0,memberId=0,browser;
const results=[];
async function shot(page,name) { await page.screenshot({path:path.join(out,name+'.png'),fullPage:true}); }
async function check(name,pass) { results.push({name,pass}); if(!pass) throw new Error(name); }
(async()=>{
 try {
    typeId=Number(sql("INSERT INTO FacilityType(TypeName) VALUES ('"+tag+"'); SELECT SCOPE_IDENTITY();"));
    facilityId=Number(sql("INSERT INTO Facility(Name,TypeID,Location,Capacity,HourlyRate) VALUES ('Report Tennis Court',"+typeId+",'Report Test Area',4,1500); SELECT SCOPE_IDENTITY();"));
    browser=await chromium.launch({channel:'msedge',headless:true});
    const context=await browser.newContext({viewport:{width:1040,height:720}});
    const page=await context.newPage();
    const base='http://127.0.0.1:5187';
    await page.goto(base); await shot(page,'home-guest');
    await page.goto(base+'/Facility/Search'); await shot(page,'search-guest');
    await page.goto(base+'/Booking/Create');
    await check('Guest redirected from booking form',page.url().endsWith('/Account/Login'));
    await shot(page,'guest-blocked');
    await page.locator('#email').fill('invalid@example.test'); await page.locator('#password').fill('wrong');
    await page.getByRole('button',{name:'Login',exact:true}).click();
    await check('Invalid login rejected',await page.getByText('Invalid email or password.').isVisible()); await shot(page,'login-error');
    await page.goto(base+'/Account/Register');
    await page.locator('#Name').fill('Report Evidence Member'); await page.locator('#Email').fill(email);
    await page.locator('#Phone').fill('0770000000'); await page.locator('#Address').fill('Report Test Area');
    await page.locator('#Password').fill('Test123!');
    await page.locator('input[name="SelectedSports"][value="'+typeId+'"]').check();
    await shot(page,'registration');
    await page.getByRole('button',{name:'Register',exact:true}).click();
    await check('Registration opens member search',page.url().includes('/Facility/Search'));
    memberId=Number(sql("SELECT MemberID FROM Member WHERE Email='"+email+"';"));
    const day = new Date(); day.setDate(day.getDate()+30); const date=day.toISOString().slice(0,10);
    await page.goto(base+'/Facility/Search?Location=Report%20Test%20Area&Date='+date+'&StartTime=10:00&EndTime=11:00');
    await shot(page,'search-member');
    await page.goto(base+'/Booking/Create?facilityId='+facilityId);
    await page.locator('#BookingDate').fill(date); await page.locator('#StartTime').fill('10:00'); await page.locator('#EndTime').fill('11:00');
    await shot(page,'booking-form'); await page.getByRole('button',{name:'Request booking'}).click();
    await check('Booking confirmation displayed',await page.getByText('Your booking request has been saved.').isVisible()); await shot(page,'booking-saved');
    await page.goto(base+'/Booking/Create?facilityId='+facilityId);
    await page.locator('#BookingDate').fill(date); await page.locator('#StartTime').fill('10:30'); await page.locator('#EndTime').fill('11:30');
    await page.getByRole('button',{name:'Request booking'}).click();
    await check('Overlapping booking rejected',await page.getByText('This facility is already booked during that time.',{exact:true}).isVisible()); await shot(page,'booking-clash');
    await page.goto(base+'/Facility/Search?TypeId='+typeId+'&Date='+date+'&StartTime=10:30&EndTime=11:30');
    await check('Booked slot absent from search',await page.getByText('No facilities match your search.').isVisible()); await shot(page,'search-unavailable');
    sql("INSERT INTO Booking(MemberID,FacilityID,BookingDate,StartTime,EndTime,Status) VALUES ("+memberId+","+facilityId+",'2020-01-01','2020-01-01T10:00:00','2020-01-01T11:00:00','Confirmed');");
    await page.goto(base+'/Review/Create'); await page.locator('#FacilityId').selectOption(String(facilityId)); await page.locator('#Rating').fill('4');
    await page.locator('#Comments').fill('The court was clean and the booking process was easy.');
    await shot(page,'review-form'); await page.getByRole('button',{name:'Submit review'}).click();
    await check('Review saved',await page.getByText('Thank you. Your review has been saved.').isVisible());
    await page.goto(base+'/Review?facilityId='+facilityId+'&rating=4'); await shot(page,'review-saved');
    const guestContext=await browser.newContext({viewport:{width:1040,height:720}}); const guest=await guestContext.newPage();
    await guest.goto(base+'/Review?facilityId='+facilityId+'&rating=4'); await shot(guest,'review-guest');
    await check('Guest reads saved review',await guest.getByText('The court was clean and the booking process was easy.').isVisible());
    await guest.goto(base+'/Inquiry/Create'); await guest.locator('#GuestName').fill('Report Evidence Guest');
    await guest.locator('#Email').fill(email); await guest.locator('#Message').fill('Are tennis coaching sessions available on weekends?');
    await shot(guest,'inquiry-form'); await guest.getByRole('button',{name:'Send inquiry'}).click();
    await check('Inquiry confirmation displayed',await guest.getByText('Your inquiry has been sent to the Sports Council.').isVisible()); await shot(guest,'inquiry-saved');
    snapshot.evidenceRows = {
        member:json("SELECT MemberID,Name,Email,RegDate FROM Member WHERE MemberID="+memberId),
        preference:json("SELECT s.MemberID,t.TypeName FROM SportPreference s JOIN FacilityType t ON s.TypeID=t.TypeID WHERE s.MemberID="+memberId),
        booking:json("SELECT BookingID,MemberID,FacilityID,BookingDate,StartTime,EndTime,Status FROM Booking WHERE MemberID="+memberId),
        review:json("SELECT ReviewID,MemberID,FacilityID,Rating,Comments,ReviewDate FROM Review WHERE MemberID="+memberId),
        inquiry:json("SELECT InquiryID,GuestName,Message,DateSent FROM Inquiry WHERE Email='"+email+"'")
    };
    fs.writeFileSync(path.join(out,'database.json'),JSON.stringify(snapshot,null,2));
    const escape=s=>String(s??'').replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;');
    async function tableShot(title,rows,file) {
        const cols=Object.keys(rows[0]||{Result:''});
        await page.setContent('<html><head><style>body{font:16px Arial;margin:30px;color:#111}h1{font-size:23px}table{border-collapse:collapse;width:100%;font-size:15px}td,th{border:1px solid #bbb;padding:9px;text-align:left}th{background:#eee}p{color:#555}</style></head><body><h1>'+escape(title)+'</h1><p>SportsBooking · SQL Server result captured '+new Date().toISOString().slice(0,10)+'</p><table><tr>'+cols.map(c=>'<th>'+escape(c)+'</th>').join('')+'</tr>'+rows.map(r=>'<tr>'+cols.map(c=>'<td>'+escape(r[c])+'</td>').join('')+'</tr>').join('')+'</table></body></html>');
        await page.screenshot({path:path.join(out,file+'.png'),fullPage:true});
    }
    for(let i=0;i<snapshot.queries.length;i++) await tableShot(snapshot.queries[i].title,snapshot.queries[i].rows,'query-'+(i+1));
    await tableShot('Table row counts',snapshot.counts,'table-counts');
    await tableShot('Database check constraints',snapshot.checks,'constraints');
    for(const [name,rows] of Object.entries(snapshot.evidenceRows)) await tableShot('Saved '+name+' data',rows,'data-'+name);
    fs.writeFileSync(path.join(out,'browser-tests.json'),JSON.stringify(results,null,2));
    console.log(results.length+' browser checks passed; screenshots saved.');
 } finally {
    sql("DELETE FROM Review WHERE FacilityID="+facilityId+"; DELETE FROM Booking WHERE FacilityID="+facilityId+"; DELETE FROM SportPreference WHERE MemberID IN (SELECT MemberID FROM Member WHERE Email='"+email+"'); DELETE FROM Inquiry WHERE Email='"+email+"'; DELETE FROM Member WHERE Email='"+email+"'; DELETE FROM Facility WHERE FacilityID="+facilityId+"; DELETE FROM FacilityType WHERE TypeID="+typeId+";");
    if(browser) await browser.close();
    console.log('Temporary screenshot records removed.');
 }
})().catch(e=>{console.error(e);process.exitCode=1;});
