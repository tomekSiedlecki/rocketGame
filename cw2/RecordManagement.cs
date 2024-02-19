using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class RecordManagement
    {
        static public void addRecordIfNew(User currentUser, int newRecord)
        {
            using (var context = new Context())
            { 
                Record tmp = context.Records.FirstOrDefault(x => x.User.Id == currentUser.Id);
                if (tmp != null)
                {
                    if(tmp.Score <  newRecord)
                    {
                        tmp.Score = newRecord;
                    }
                }
                else
                {
                    Record newRecordForUser = new Record();
                    newRecordForUser.Score = newRecord;
                    User user = context.Users.FirstOrDefault(x => x.Id == currentUser.Id);
                    newRecordForUser.User = user;
                    context.Records.Add(newRecordForUser);
                }
                context.SaveChanges();
            }
        }

        static public void DisplayAllRecords(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("Aby oposcic nacisnij jakikolwiek przycisk");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Wszystkie rekordy", Console.ForegroundColor);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();

            using (var context = new Context())
            {
                List<Record> allRecords = context.Records.Include(r => r.User).ToList();
                allRecords = allRecords.OrderBy(r => r.Score).ToList();
                foreach(Record row in allRecords)
                {
                    if(currentUser.Id == row.User.Id)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(row.ToString(), Console.ForegroundColor);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine(row.ToString());
                    }
                }
            }
            ConsoleKeyInfo keyInfo = Console.ReadKey();
        }
    }
}


