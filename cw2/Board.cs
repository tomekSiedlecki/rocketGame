using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class Board
    {
        private Entity[] objects;
        private int height;
        private int width;
        private string rocketString = "^";
        private string boardString = " ";
        public int rocketX;
        public int rocketY;
        private readonly object lockObjectMove = new object();
        private readonly object lockObjectClose = new object();
        private readonly object lockObjectDisplay = new object();
        private readonly object lockObjectSpawn = new object();
        private readonly object lockObject = new object();
        public bool ifclose = false;
        public int score;
        private Dictionary<int, string> asteroidTypes;

        public Board(int height = 10, int width = 7, int score = 0)
        {
            this.height = height;
            this.width = width;
            this.rocketY = height -1 -1;
            this.rocketX = width / 2;
            objects = new Entity[0];
            this.score = score;

            asteroidTypes = new Dictionary<int, string>();
            asteroidTypes.Add(1, "*");
        }
        public void DisplayWithColor()
        {
            string upDown = new string('═', width + 2);
            upDown = "╔" + upDown.Substring(1, upDown.Length - 2) + "╗";
            lock (lockObjectDisplay)
            {
                Console.Clear();
                Console.WriteLine(upDown);
            }
            for (int i = 0; i < height; i++)
            {
                string result = "║";
                for (int j = 0; j < width; j++)
                {
                    Entity objectEncountered = null;
                    foreach (var obj in objects)
                    {
                        if (obj.X == j && obj.Y == i)
                        {
                            objectEncountered = obj;
                            break;
                        }
                    }

                    //rakieta to dodaje do linii
                    if (rocketX == j && rocketY == i)
                    {
                        Console.Write(result);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(rocketString, Console.ForegroundColor);
                        Console.ForegroundColor = ConsoleColor.White;
                        result = "";
                    }
                    //jezeli jest obiekt to dodaje do linii
                    else if (objectEncountered != null)
                    {
                        Console.Write(result);
                        if (objectEncountered is Asteroid)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (objectEncountered is Bomb)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Console.Write(objectEncountered.boardString, Console.ForegroundColor);
                        Console.ForegroundColor = ConsoleColor.White;
                        result = "";
                    }
                    else
                    {
                        result += boardString;
                    }
                }
                result += "║";

                Console.WriteLine(result);

            }
            upDown = "╚" + upDown.Substring(1, upDown.Length - 2) + "╝";
            lock (lockObjectDisplay)
            {
                Console.ResetColor();
                Console.WriteLine(upDown);

                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("SCORE " + score);
            }
        }

        public void DisplayNoColor()
        {
            string upDown = new string('-', width + 2);
            lock (lockObjectDisplay)
            {
                Console.Clear();
                Console.WriteLine(upDown);
            }
            for (int i = 0; i < height; i++)
            {
                string result = "|";
                for (int j = 0; j < width; j++)
                {
                    bool ifObject = false;
                    string objectString = " ";
                    foreach (var obj in objects)
                    {
                        if (obj.X == j && obj.Y == i)
                        {
                            ifObject = true;
                            objectString = obj.boardString;
                            break;
                        }
                    }
                    if (rocketX == j && rocketY == i)
                    {
                        result += rocketString;
                    }
                    else if (ifObject)
                    {
                        result += objectString;
                    }
                    else
                    {
                        result += boardString;
                    }
                }
                result += "|";

                Console.WriteLine(result);
            }
            lock (lockObjectDisplay)
            {
                Console.ResetColor();
                Console.WriteLine(upDown);

                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("SCORE " + score);
            }
        }

        private Entity[] getObjectsLine(int y)
        {
            int size = 0;
            foreach (var obj in objects)
            {
                if(obj.Y == y)
                {
                    size++;
                }
            }
            Entity[] result = new Entity[size];
            int i = 0;
            foreach (var obj in objects)
            {
                if (obj.Y == y)
                {
                    result[i] = obj;
                }
            }
            return result;
        }

        public void Left()
        {
            lock (lockObjectMove)
            {
                if (rocketX > 0)
                {
                    rocketX--;
                }
            }
        }
        public void Right()
        {
            lock (lockObjectMove)
            {
                if (rocketX < width - 1)
                {
                    rocketX++;
                }
            }
        }

        public void SpawnObjects(int amountAsteroids, int amountBombs)
        {
            Random random = new Random();
            int countAsteroids = amountAsteroids;
            int countBombs = amountBombs;

            //suma ilosci wszystkich obiektow
            int countAllObjects = countAsteroids + countBombs;

            int[] randomPlaces = new int[countAllObjects];

            //ustawienie wszytkich na -1 czyli bez Y
            for (int i = 0; i < countAllObjects; i++)
            {
                randomPlaces[i] = -1;
            }

            //wylosowanie unikatowych miejsc tyle ile wskazuje wylosowana ilosc wszystkich obiektow
            for (int i = 0; i < countAllObjects; i++)
            {
                int place;
                do
                {
                    //losowa koordynata
                    place = random.Next(0, width);

                } while (IfInArray(place, randomPlaces));
                randomPlaces[i] = place;
            }

            lock (lockObjectSpawn)
            {
                //dodanie wartosci do tablicy
                int oldSize = objects.Length;
                int newSize = objects.Length + countAllObjects;
                Entity[] result = new Entity[newSize];

                int j = 0;

                //stare
                for (int i = 0; i < oldSize; i++)
                {
                    result[j] = objects[i];
                    j++;
                }

                //nowe
                int counter;
                for (counter = 0; counter < countAsteroids; counter++)
                {
                    result[j] = new Asteroid(randomPlaces[counter], 0, randomAsteroidType());
                    j++;
                }
                for(; counter < countAllObjects; counter++)
                {
                    result[j] = new Bomb(randomPlaces[counter], 0);
                    j++;
                }
                objects = result;
            }
        }

        public void DisplayObjects()
        {
            foreach (var item in objects)
            {
                Console.WriteLine(item.boardString + "=   X: " + item.X + " Y: " + item.Y);
            }
            Console.WriteLine("Rocket" + "=   X: " + rocketX + " Y: " + rocketY);
        }

        public string randomAsteroidType()
        {
            //Random random = new Random();
            //return asteroidTypes[random.Next(1, asteroidTypes.Count + 1)];
            //return "*";
            return "▒";
        }


        public bool IfInArray(int value, int[] randomPlaces)
        {
            bool result = false;
            foreach (int place in randomPlaces)
            {
                if (place == value) { result = true; }
            }
            return result;
        }

        public void MoveObjects()
        {
            List<int> objectsToRemove = new List<int>();
            for (int i = 0; i < objects.Length; i++)
            {
                //asteroida na ostatni polu i ma znikac
                if (objects[i].Y == height - 1)
                {
                    objectsToRemove.Add(i);
                }
                else
                {
                    objects[i].Y++;
                }
            }

            int newSize = objects.Length - objectsToRemove.Count;
            Entity[] result = new Entity[newSize];
            int resultIndex = 0;
            for (int i = 0; i < objects.Length; i++)
            {

                //--------czy usunac -------------
                bool ifDelete = false;
                foreach (int index in objectsToRemove)
                {
                    if (index == i)
                    {
                        ifDelete = true;
                    }
                }
                //----------------------------

                //dodaje jezeli nie jest do usuniecia
                if (!ifDelete)
                {
                    result[resultIndex] = objects[i];
                    resultIndex++;
                }

            }
            objects = result;
        }

        public bool onContact()
        {
            bool result = true;

            foreach (var objectEntity in objects)
            {
                if (objectEntity.X == rocketX && objectEntity.Y == rocketY)
                {
                    result = objectEntity.onContact(this);
                    break;
                }
            }
            return result;
        }

        public void RemoveCloseObjects(int x, int y)
        {
            int range = 2;

            int counter = 0;
            foreach (var item in objects)
            {
                //czy X i Y jest w zasiegu 2
                if ((item.X - x <= range && item.X - x >= -range) && (item.Y - y <= range && item.Y - y >= -range))
                {
                    counter++;
                }
            }

            Entity[] result = new Entity[objects.Length - counter];

            int counter_next = 0;
            foreach (var item in objects)
            {
                //czy X i Y jest w zasiegu 2
                if (!((item.X - x <= range && item.X - x >= -range) && (item.Y - y <= range && item.Y - y >= -range)))
                {
                    result[counter_next] = item;
                    counter_next++;
                }
            }

            lock (lockObject)
            {
                objects = result;
            }
        }
    }
}
