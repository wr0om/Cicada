using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Lambda2
{
    /// <summary>
    /// Handles sqlite db of the app
    /// </summary>
    class SqlDb : Data //Retains all data until uninstall
    {
        private SQLiteConnection dbConnection;
        /// <summary>
        /// constructor, creates a questions table.
        /// </summary>
        public SqlDb() : base(Constants.SQL_DATA_FILENAME)
        {
            OpenConnection();
            dbConnection.CreateTable<Question>();
        }
        /// <summary>
        /// Inserts question to the questions table in sqlite
        /// </summary>
        /// <param name="questions">question object</param>
        public void InsertQuestion(Question questions)
        {
            dbConnection.Insert(questions);
        }
        /// <summary>
        /// returns a list of all the questions in the sqlite table
        /// </summary>
        /// <returns></returns>
        public List<Question> GetAllQuestions()
        {
            return dbConnection.Table<Question>().ToList();
        }
        /// <summary>
        /// closes the sql db connection
        /// </summary>
        public void Close()
        {
            dbConnection.Close();
        }
        /// <summary>
        /// opens the sql connection
        /// </summary>
        private void OpenConnection()
        {
            dbConnection = new SQLiteConnection(DbPath);
        }
        /// <summary>
        /// Initializes the questions in the database
        /// </summary>
        public void InitQuestionsDB()
        {
            Question[] questions = new Question[Constants.NUMBER_OF_QUESTIONS];
            for (int i = 0; i < questions.Length; i++)
            {
                questions[i] = new Question();
            }
            questions[0].Que = "מכמה צבעים מורכב הלוגו של גוגל?";
            questions[1].Que = "מהו המספר הפעמים המקסימלי שבהן יכול לחול יום השישי ה-13 בשנה?";
            questions[2].Que = "באיזו שנה המריאה הטיסה הראשונה של אל-על? ";
            questions[3].Que = "באיזו שנה הגיחה בובת הברבי הראשונה לעולם?";
            questions[4].Que = "כל סיגריה מכניסה _ _ חומרים מסרטנים לגופך";
            questions[5].Que = "כמה מילים מכילים ראשי התיבות נדלן";
            questions[6].Que = "מה מספר המשתתפים המקסימלי שקבוצת וואטסאפ יכולה להכיל?";
            questions[7].Que = "כמה רגליים יש לפרפר? ";
            questions[8].Que = "מהו מספר הכביש הארוך ביותר בישראל?";
            questions[9].Que = "כמה אותיות אהוי יש בשאלה הזו?";
            questions[10].Que = "מה ערכו של השטר המזוהה עם זלמן שזר?";
            questions[11].Que = "כמה אבני-משחק יש במשחק שש-בש?";
            questions[12].Que = "אם ארצה להזמין משטרה בלונדון – איזה מספר יהיה עליי לחייג?";
            questions[13].Que = "איזה מספר אחייג אם ברצוני להשיג את פיקוד העורף?";
            questions[14].Que = "כמה אותיות יש בשפה האנגלית?";
            questions[15].Que = "באיזה שנה הושק לראשונה שירות ההודעות הפופולארי וואטסאפ?";
            questions[16].Que = "באיזה שנה הוצג לראשונה החטיף פסק זמן?";
            questions[17].Que = "בן כמה האיש הזקן בעולם?";
            questions[18].Que = "הציון המינימלי שניתן לקבל בבחינה הפסיכומטרית.";
            questions[19].Que = "מטבע של 10 שקלים שוקל _ גרם. ";
            questions[20].Que = "באיזו שנה שינתה הולנד את החקיקה בנושא הקנאביס?";
            questions[21].Que = "כמה אצבעות יש בכף יד של שימפנזה?";
            questions[22].Que = "באיזו שנה הותר לנשים לנהוג ברכב בערב הסעודית?";
            questions[23].Que = "באיזו שנה התפרקה ברית המועצות?";
            questions[24].Que = "כמה צבעים יש בדגל וייטנאם?";
            questions[25].Que = "לפי שיאי גינס – כמה קג שקל האדם השמן ביותר בהיסטוריה?";
            questions[26].Que = "כמה צבעים יש בדגל צ'כיה?";
            questions[27].Que = "כמה קווים ונקודות צריך כדי לכתוב את המילה אהבה בקוד מורס עברי?";
            questions[28].Que = "כמה מבניו של המן נתלו לפי המתואר במגילת אסתר?";
            questions[29].Que = "כמה חברי כנסת יש בכנסת ישראל?";
            questions[30].Que = "כמה שבועות נמשך הריון ממוצע של אישה?";
            questions[31].Que = "כמה עמודים ישנם בכל שבעת ספרי הארי פוטר?";
            questions[32].Que = "באיזו שנה יצא לראשונה סופרמן על חוברות הקומיקס?";
            questions[33].Que = "בשנת 1802 אוכלוסיית העולם עמדה על _ מיליארד אנשים.";
            questions[34].Que = "כמה אצבעות בחטיף קיט קט קלאסי?";
            questions[35].Que = "כמה חורים יש בטי-שירט?";
            questions[36].Que = "מה מספר העיניים המירבי שיש לעכבישים?";
            questions[37].Que = "כמה רובעים יש בעיר העתיקה בירושלים?";
            questions[38].Que = "כמה שערים פתוחים ישנם מסביבה של העיר העתיקה בירושלים?";
            questions[39].Que = "כמה ספרים קיימים בסדרת הארי פוטר?";
            questions[40].Que = "בפיוט של ערב שבת שלום עליכם – כמה פעמים כתובה המילה שלום?";
            questions[41].Que = "בסך הכל, כמה אנשים חתמו על מגילת העצמאות של מדינת ישראל?";
            questions[42].Que = "המלכה הנוכחית של בריטניה, היא המלכה אליזבת ה-_.";
            questions[43].Que = "כמה חדרי שינה יש במלון הכי גדול בעולם?";
            questions[44].Que = "כמה דקות יש ברבע שעה?";
            questions[45].Que = "בכמה זיתים בממוצע יהיה עליי להשתמש כדי להכין ליטר שמן זית?";
            questions[46].Que = "בכמה צבעים לבחירה מגיע האימוג'י של האוזן במקלדת?";
            questions[47].Que = "שם של מפציץ אמריקאי: _ _ B. ";
            questions[48].Que = "הנקודה ההכי עמוקה הידועה מתחת לפני הים במטרים.";
            questions[49].Que = "מכמה מחומשים בנוי הפנטגון?";
            questions[50].Que = "באיזה לילה בדצמבר מתחיל הסילבסטר?";
            questions[51].Que = "כמה דקות יש ביממה?";
            questions[52].Que = "נכון לשנת 2015 – כמה סניפים יש לרשת פיצה האט ברחבי העולם?";
            questions[53].Que = "כמה שנים לקח לבנות את הטאג' מאהל?";
            questions[54].Que = "נכון לשנת 2018, כמה מדליות אולימפיות יש לישראל?";
            questions[55].Que = "כמה שופטים מכהים בבית המשפט העליון?";
            questions[56].Que = "כמות המצוות, כמו גרעינים ברימון";
            questions[57].Que = "מכמה חברות הורכבה החבר הוט?";
            questions[58].Que = "באיזו שנה הכירו האומות המאוחדות במדינת ישראל?";
            questions[59].Que = "בן כמה מתושלח, העץ החי הזקן בעולם?";
            questions[60].Que = "נכון לשנת 2017, מה הוא שיא מספר שכיבות הסמיכה בשעה?";
            questions[61].Que = "מהו אורכו במטרים של גשר הזהב?";
            questions[62].Que = "כמה פרוטות לפחות חייבת להיות שווה טבעת נישואין?";
            questions[63].Que = "לוי אשכול היה ראש הממשלה ה-_ של מדינת ישראל.";
            questions[64].Que = "באיזו שנה, נכנס לפעילות כפתור הלייק בפייסבוק?";
            questions[65].Que = "לפי סיפור הבריאה, העצים נבראו ביום ה-_י";
            questions[66].Que = "בטאקי, הקלפים כוללים ספרות מהספרה 1 עד _";
            questions[67].Que = "ג'ירף יכול לרוץ עד _ _ קילומטר לשעה";
            questions[68].Que = "סמל אינסוף דומה לספרה זו שוכבת על צידה";
            questions[69].Que = "כמה שרירים מופעלים בכל צעד אחד שלנו?";
            questions[70].Que = "נכון ל2018, גיל הפרישה של נשים הוא _ _ שנים";
            questions[71].Que = "באיזו שנה נוסדה מייקרוסופט?";
            questions[72].Que = "באיזו שנה נבנו מגדלי התאומים?";
            questions[73].Que = "לגובה של כמה סנטימטרים מתנשאת בר רפאלי?";
            questions[74].Que = "יום צום בחודש אב";
            questions[75].Que = "כמה משולשים נראים לעין יש במגן דוד?";
            questions[76].Que = "ביום ה-_ _ לספירת העומר - חוגגים את חג השבועות";
            questions[77].Que = "במקלדת במחשב, איזה מספר מופיע על סימן הדולר?";
            questions[78].Que = "כמה מילים שוות תמונה אחת?";
            questions[79].Que = "מיניבוס הוא כלי רכב להסעת נוסעים שמספר מושביו עד _ _ מקומות";
            questions[80].Que = "באיזה גיל זכתה הזוכה הצעירה ביותר, מלאלה יוספזאי, בפרס נובל?";
            questions[81].Que = "בממוצע, כמה מילים יודע לבטא ילד בן 6?";
            questions[82].Que = "כמה תחנות רכבת יש בבאר-שבע?";
            questions[83].Que = "כמה ערי בירה יש בדרום אפריקה?";
            questions[84].Que = "גדוד _ _ של גולני, ידוע גם בשמו - הבוקעים הראשונים";
            questions[85].Que = "כמה צלעות יש למטבע של 5 שקלים?";
            questions[86].Que = "נכון לתחילת 2017, כמה שירים יש למטליקה?";
            questions[87].Que = "חתול תמיד נוחת על _ רגליים";
            questions[88].Que = "כמה שנים כיהן רמה ה-9 מלך תאילנד?";
            questions[89].Que = "כמה סימנים (אותיות) היו בכתב העברי הקדום?";
            questions[90].Que = "בשיא כוחו, דורג פבלו אסקובר במקום ה-_ מבין עשירי העולם";
            questions[91].Que = "באיזו שנה הוקמה להקת הרוק הגרמנית הסקורפיונס?";
            questions[92].Que = "אם 25 קרטוני חלב עולים 200 שקלים, כמה יעלו 3 קרטונים?";
            questions[93].Que = "באיזו שנה נפטר שייקספיר?";
            questions[94].Que = "כמה תלמידים בשכבה עם 7 כיתות כשבכל כיתה 34 תלמידים?";
            questions[95].Que = "מה הערך של האות י' בגימטריה?";
            questions[96].Que = "ראובן ריבלין הינו הנשיא ה_ _ של מדינת ישראל";
            questions[97].Que = "כמה כוכבים יש בדגל האיחוד האירופאי?";
            questions[98].Que = "לאן מתקשרים אם מחפשים את מספר הטלפון של מישהו?";
            questions[99].Que = "כמה פעמים, בקירוב נכנס קוטר המעגל בהיקפו?";
            

            questions[0].Ans = 4;
            questions[1].Ans = 3;
            questions[2].Ans = 1948;
            questions[3].Ans = 1959;
            questions[4].Ans = 43;
            questions[5].Ans = 3;
            questions[6].Ans = 256;
            questions[7].Ans = 6;
            questions[8].Ans = 90;
            questions[9].Ans = 14;
            questions[10].Ans = 200;
            questions[11].Ans = 30;
            questions[12].Ans = 999;
            questions[13].Ans = 104;
            questions[14].Ans = 26;
            questions[15].Ans = 2009;
            questions[16].Ans = 1982;
            questions[17].Ans = 145;
            questions[18].Ans = 200;
            questions[19].Ans = 7;
            questions[20].Ans = 1976;
            questions[21].Ans = 5;
            questions[22].Ans = 2018;
            questions[23].Ans = 1991;
            questions[24].Ans = 2;
            questions[25].Ans = 635;
            questions[26].Ans = 3;
            questions[27].Ans = 12;
            questions[28].Ans = 10;
            questions[29].Ans = 120;
            questions[30].Ans = 40;
            questions[31].Ans = 3407;
            questions[32].Ans = 1938;
            questions[33].Ans = 1;
            questions[34].Ans = 4;
            questions[35].Ans = 4;
            questions[36].Ans = 8;
            questions[37].Ans = 4;
            questions[38].Ans = 7;
            questions[39].Ans = 7;
            questions[40].Ans = 8;
            questions[41].Ans = 37;
            questions[42].Ans = 2;
            questions[43].Ans = 7351;
            questions[44].Ans = 15;
            questions[45].Ans = 1000;
            questions[46].Ans = 6;
            questions[47].Ans = 52;
            questions[48].Ans = 11034;
            questions[49].Ans = 5;
            questions[50].Ans = 31;
            questions[51].Ans = 1440;
            questions[52].Ans = 15000;
            questions[53].Ans = 20;
            questions[54].Ans = 9;
            questions[55].Ans = 15;
            questions[56].Ans = 613;
            questions[57].Ans = 3;
            questions[58].Ans = 1947;
            questions[59].Ans = 4765;
            questions[60].Ans = 2682;
            questions[61].Ans = 2737;
            questions[62].Ans = 1;
            questions[63].Ans = 3;
            questions[64].Ans = 2009;
            questions[65].Ans = 3;
            questions[66].Ans = 9;
            questions[67].Ans = 60;
            questions[68].Ans = 8;
            questions[69].Ans = 200;
            questions[70].Ans = 62;
            questions[71].Ans = 1975;
            questions[72].Ans = 1973;
            questions[73].Ans = 174;
            questions[74].Ans = 9;
            questions[75].Ans = 8;
            questions[76].Ans = 50;
            questions[77].Ans = 4;
            questions[78].Ans = 1000;
            questions[79].Ans = 20;
            questions[80].Ans = 17;
            questions[81].Ans = 2600;
            questions[82].Ans = 2;
            questions[83].Ans = 3;
            questions[84].Ans = 51;
            questions[85].Ans = 12;
            questions[86].Ans = 175;
            questions[87].Ans = 4;
            questions[88].Ans = 70;
            questions[89].Ans = 22;
            questions[90].Ans = 7;
            questions[91].Ans = 1965;
            questions[92].Ans = 24;
            questions[93].Ans = 1616;
            questions[94].Ans = 238;
            questions[95].Ans = 10;
            questions[96].Ans = 10;
            questions[97].Ans = 12;
            questions[98].Ans = 144;
            questions[99].Ans = 3;

            for (int i = 0; i < questions.Length; i++)
            {
                InsertQuestion(questions[i]);
            }
        }
    }
}