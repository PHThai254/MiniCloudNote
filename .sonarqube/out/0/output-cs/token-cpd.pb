¿
M/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/TaxCalculator.cs
	namespace 	
MiniCloudNote
 
. 
Core 
; 
public 
class 
TaxCalculator 
{ 
public 

decimal 
CalculateTax 
(  
decimal  '
income( .
). /
{ 
if 

( 
income 
> 
$num 
)  
{		 	
return

 
income

 
*

 
$num

  
;

  !
} 	
return 
$num 
; 
} 
} ç9
T/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Services/NoteService.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Services %
{ 
public		 

class		 
NoteService		 
:		 
INoteService		 +
{

 
private 
readonly 
IEnumerable $
<$ %
IFormattingStrategy% 8
>8 9!
_formattingStrategies: O
;O P
private 
readonly 
INoteRepository (
_noteRepository) 8
;8 9
public 
NoteService 
( 
IEnumerable &
<& '
IFormattingStrategy' :
>: ; 
formattingStrategies< P
,P Q
INoteRepositoryR a
noteRepositoryb p
)p q
{ 	!
_formattingStrategies !
=" # 
formattingStrategies$ 8
;8 9
_noteRepository 
= 
noteRepository ,
;, -
} 	
public 
async 
Task 
< 
Note 
> 
CreateNoteAsync  /
(/ 0
string0 6
title7 <
,< =
string> D
contentE L
)L M
{ 	
if 
( 
string 
. 
IsNullOrEmpty $
($ %
title% *
)* +
)+ ,
{ 
throw 
new 
ArgumentException +
(+ ,
$str, B
)B C
;C D
}   
if!! 
(!! 
content!! 
?!! 
.!! 
Length!! 
>!!  !
$num!!" &
)!!& '
{"" 
throw## 
new## 
ArgumentException## +
(##+ ,
$str##, ?
)##? @
;##@ A
}$$ 
var(( 
newNote(( 
=(( 
new(( 
Note(( "
{)) 
Id** 
=** 
Guid** 
.** 
NewGuid** !
(**! "
)**" #
,**# $
Title++ 
=++ 
title++ 
,++ 
Content,, 
=,, 
content,, !
,,,! "
	CreatedAt-- 
=-- 
DateTime-- $
.--$ %
UtcNow--% +
,--+ ,
	UpdatedAt.. 
=.. 
DateTime.. $
...$ %
UtcNow..% +
}// 
;// 
var66 
createdNote66 
=66 
await66 #
_noteRepository66$ 3
.663 4
	SaveAsync664 =
(66= >
newNote66> E
)66E F
;66F G
return<< 
createdNote<< 
;<< 
}== 	
public@@ 
async@@ 
Task@@ 
<@@ 
IEnumerable@@ %
<@@% &
Note@@& *
>@@* +
>@@+ ,
GetAllNotesAsync@@- =
(@@= >
)@@> ?
{AA 	
returnBB 
awaitBB 
_noteRepositoryBB (
.BB( )
GetAllAsyncBB) 4
(BB4 5
)BB5 6
;BB6 7
}CC 	
publicEE 
asyncEE 
TaskEE 
<EE 
NoteEE 
?EE 
>EE  
GetNoteByIdAsyncEE! 1
(EE1 2
GuidEE2 6
idEE7 9
)EE9 :
{FF 	
returnGG 
awaitGG 
_noteRepositoryGG (
.GG( )
GetByIdAsyncGG) 5
(GG5 6
idGG6 8
)GG8 9
;GG9 :
}HH 	
publicKK 
asyncKK 
TaskKK 
UpdateNoteAsyncKK )
(KK) *
GuidKK* .
idKK/ 1
,KK1 2
stringKK3 9
titleKK: ?
,KK? @
stringKKA G
contentKKH O
)KKO P
{LL 	
varNN 
existingNoteNN 
=NN 
awaitNN $
_noteRepositoryNN% 4
.NN4 5
GetByIdAsyncNN5 A
(NNA B
idNNB D
)NND E
;NNE F
ifOO 
(OO 
existingNoteOO 
==OO 
nullOO  $
)OO$ %
{PP 
throwQQ 
newQQ  
KeyNotFoundExceptionQQ .
(QQ. /
$strQQ/ O
)QQO P
;QQP Q
}RR 
existingNoteUU 
.UU 
TitleUU 
=UU  
titleUU! &
;UU& '
existingNoteVV 
.VV 
ContentVV  
=VV! "
contentVV# *
;VV* +
existingNoteWW 
.WW 
	UpdatedAtWW "
=WW# $
DateTimeWW% -
.WW- .
UtcNowWW. 4
;WW4 5
awaitZZ 
_noteRepositoryZZ !
.ZZ! "
UpdateAsyncZZ" -
(ZZ- .
existingNoteZZ. :
)ZZ: ;
;ZZ; <
}[[ 	
public^^ 
async^^ 
Task^^ 
DeleteNoteAsync^^ )
(^^) *
Guid^^* .
id^^/ 1
)^^1 2
{__ 	
varaa 
existingNoteaa 
=aa 
awaitaa $
_noteRepositoryaa% 4
.aa4 5
GetByIdAsyncaa5 A
(aaA B
idaaB D
)aaD E
;aaE F
ifbb 
(bb 
existingNotebb 
==bb 
nullbb  $
)bb$ %
{cc 
throwdd 
newdd  
KeyNotFoundExceptiondd .
(dd. /
$strdd/ O
)ddO P
;ddP Q
}ee 
awaithh 
_noteRepositoryhh !
.hh! "
DeleteAsynchh" -
(hh- .
existingNotehh. :
)hh: ;
;hh; <
}ii 	
publicjj 
stringjj 
FormatNoteContentjj '
(jj' (
stringjj( .
contentjj/ 6
,jj6 7
stringjj8 >

formatTypejj? I
)jjI J
{kk 	
varmm 
strategymm 
=mm !
_formattingStrategiesmm 0
.mm0 1
FirstOrDefaultmm1 ?
(mm? @
smm@ A
=>mmB D
smmE F
.mmF G

FormatTypemmG Q
==mmR T

formatTypemmU _
)mm_ `
;mm` a
ifoo 
(oo 
strategyoo 
!=oo 
nulloo  
)oo  !
{pp 
returnrr 
strategyrr 
.rr  
Formatrr  &
(rr& '
contentrr' .
)rr. /
;rr/ 0
}ss 
throwvv 
newvv !
NotSupportedExceptionvv +
(vv+ ,
$"vv, .
$strvv. 9
{vv9 :

formatTypevv: D
}vvD E
$strvvE Y
"vvY Z
)vvZ [
;vv[ \
}xx 	
public|| 
string|| 
GeneratePreview|| %
(||% &
IReadOnlyNote||& 3
note||4 8
)||8 9
{}} 	
return
€€ 
$"
€€ 
$str
€€ 
{
€€ 
note
€€ #
.
€€# $
Title
€€$ )
}
€€) *
$str
€€* -
{
€€- .
note
€€. 2
.
€€2 3
	CreatedAt
€€3 <
}
€€< =
"
€€= >
;
€€> ?
}
 	
}
‚‚ 
}ƒƒ ¤
y/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Services/FormattingStrategies/PlainTextFormattingStrategy.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Services %
.% & 
FormattingStrategies& :
{ 
public 

class '
PlainTextFormattingStrategy ,
:- .
IFormattingStrategy/ B
{ 
public 
string 

FormatType  
=>! #
$str$ /
;/ 0
public		 
string		 
Format		 
(		 
string		 #
content		$ +
)		+ ,
{

 	
return 
content 
; 
} 	
} 
} ƒ
x/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Services/FormattingStrategies/MarkdownFormattingStrategy.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Services %
.% & 
FormattingStrategies& :
{ 
public 

class &
MarkdownFormattingStrategy +
:, -
IFormattingStrategy. A
{ 
public 
string 

FormatType  
=>! #
$str$ .
;. /
public		 
string		 
Format		 
(		 
string		 #
content		$ +
)		+ ,
{

 	
return 
$" 
$str 
{ 
content 
}  
$str  "
"" #
;# $
} 	
} 
} û
t/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Services/FormattingStrategies/HtmlFormattingStrategy.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Services %
.% & 
FormattingStrategies& :
{ 
public 

class "
HtmlFormattingStrategy '
:( )
IFormattingStrategy* =
{ 
public 
string 

FormatType  
=>! #
$str$ *
;* +
public		 
string		 
Format		 
(		 
string		 #
content		$ +
)		+ ,
{

 	
return 
$" 
$str 
{ 
content  
}  !
$str! %
"% &
;& '
} 	
} 
} •
Z/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/IUserRepository.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IUserRepository $
{ 
Task 
< 
User 
? 
> 
GetByUsernameAsync &
(& '
string' -
username. 6
)6 7
;7 8
Task 
AddAsync 
( 
User 
user 
)  
;  !
}		 
}

 Á
Z/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/IStorageService.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IStorageService $
{ 
Task

 
<

 
string

 
>

 
UploadFileAsync

 #
(

# $
string

$ *
fileName

+ 3
,

3 4
Stream

5 ;

fileStream

< F
,

F G
string

H N
contentType

O Z
)

Z [
;

[ \
Task 
< 
string 
> 
GetFileUrlAsync #
(# $
string$ *
fileName+ 3
)3 4
;4 5
Task 
DeleteFileAsync 
( 
string "
fileName# +
)+ ,
;, -
} 
} é
W/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/INoteService.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
INoteService !
{ 
Task		 
<		 
Note		 
>		 
CreateNoteAsync		 "
(		" #
string		# )
title		* /
,		/ 0
string		1 7
content		8 ?
)		? @
;		@ A
Task

 
<

 
IEnumerable

 
<

 
Note

 
>

 
>

 
GetAllNotesAsync

  0
(

0 1
)

1 2
;

2 3
Task 
< 
Note 
? 
> 
GetNoteByIdAsync $
($ %
Guid% )
id* ,
), -
;- .
Task 
UpdateNoteAsync 
( 
Guid !
id" $
,$ %
string& ,
title- 2
,2 3
string4 :
content; B
)B C
;C D
Task 
DeleteNoteAsync 
( 
Guid !
id" $
)$ %
;% &
string 
FormatNoteContent  
(  !
string! '
content( /
,/ 0
string1 7

formatType8 B
)B C
;C D
} 
} ï
Z/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/INoteRepository.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
INoteRepository $
{ 
Task 
< 
Note 
> 
	SaveAsync 
( 
Note !
note" &
)& '
;' (
Task 
< 
IEnumerable 
< 
Note 
> 
> 
GetAllAsync  +
(+ ,
), -
;- .
Task 
< 
Note 
? 
> 
GetByIdAsync  
(  !
Guid! %
id& (
)( )
;) *
Task 
UpdateAsync 
( 
Note 
note "
)" #
;# $
Task 
DeleteAsync 
( 
Note 
note "
)" #
;# $
} 
} Â
W/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/INoteContent.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IReadOnlyNote "
{ 
string 
Title 
{ 
get 
; 
} 
string 
Content 
{ 
get 
; 
} 
DateTime 
	CreatedAt 
{ 
get  
;  !
}" #
}		 
public 

	interface 
IEditableNote "
:# $
IReadOnlyNote% 2
{ 
new 
string 
Title 
{ 
get 
; 
set  #
;# $
}% &
new 
string 
Content 
{ 
get  
;  !
set" %
;% &
}' (
} 
} Ä
^/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/IFormattingStrategy.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IFormattingStrategy (
{ 
string 

FormatType 
{ 
get 
;  
}! "
string 
Format 
( 
string 
content $
)$ %
;% &
}		 
}

 Ž
X/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/IEmailService.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IEmailService "
{ 
Task !
SendWelcomeEmailAsync "
(" #
string# )
email* /
,/ 0
string1 7
name8 <
)< =
;= >
} 
} °
W/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Interfaces/IAuthService.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 

Interfaces '
{ 
public 

	interface 
IAuthService !
{ 
Task 
< 
User 
> 
RegisterAsync  
(  !
User! %
user& *
,* +
string, 2
password3 ;
); <
;< =
Task 
< 
string 
? 
> 

LoginAsync  
(  !
string! '
username( 0
,0 1
string2 8
password9 A
)A B
;B C
}		 
}

 ¯
M/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Entities/User.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Entities %
{ 
public 

class 
User 
{ 
[ 	
Key	 
] 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
[

 	
Required

	 
]

 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Username 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 	
Required	 
] 
public 
string 
PasswordHash "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
FullName 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 	
Required	 
] 
public 
string 
Role 
{ 
get  
;  !
set" %
;% &
}' (
=) *
$str+ 1
;1 2
} 
} ¼
M/mnt/c/Personal Project/MiniCloudNote/src/MiniCloudNote.Core/Entities/Note.cs
	namespace 	
MiniCloudNote
 
. 
Core 
. 
Entities %
{ 
public 

class 
Note 
: 
IEditableNote %
{ 
[		 	
Key			 
]		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
public 
DateTime 
? 
	UpdatedAt "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} 