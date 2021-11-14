using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashcodeMain
{
    public class MainClass
    {
        static List<int[]> output;
        static double param;
        static int score;

        static int B;
        static int L;
        static int D;


        class book
        {
            public int id;
            public int score;
            public bool isScanned;
        }
        class library
        {
            public int nBooks;
            public int signupTime;
            public int booksPerDay;
            public book[] books;
            public int startTime;
            public int capacity;
            public List<int> booksToScan;
            public double ranking;
            public int preliminaryCapacity;
        }

        static int[] libraryOrder;


        static book[] books;
        static library[] libraries;

        public static List<int[]> Run(List<int[]> input, double param)
        {
            MainClass.param = param;
            score = 0;
            ParseList(input);
            SortBooksByScore();
            ComputePreliminaryCapacity();
            OrderLibraries();
            ComputeStartTimes();
            ComputeBooksPerLibrary();
            AssignBooksToLibraries2();
            output = GenerateOutput();
            return output;          
        }

        private static void SortBooksByScore()
        {
            for (int i = 0; i < L; i++)
            {
                library lib = libraries[libraryOrder[i]];
                Array.Sort(lib.books, (x, y) => y.score.CompareTo(x.score));
            }
        }

        private static void SortBooksById()
        {
            for (int i = 0; i < L; i++)
            {
                library lib = libraries[libraryOrder[i]];
                Array.Sort(lib.books, (x, y) => x.id.CompareTo(y.id));
            }
        }

        private static void AssignBooksToLibraries()
        {
            //for (int i = 0; i < L; i++)
            for (int i = 0; i < L; i++)
            {
                library lib = libraries[libraryOrder[i]];
                int j = 0;
                int k = 0;
                lib.booksToScan = new List<int>();
                while(j < lib.capacity && k < lib.nBooks)
                {
                    if (!lib.books[k].isScanned)
                    {
                        lib.booksToScan.Add(lib.books[k].id);
                        lib.books[k].isScanned = true;
                        score += lib.books[k].score;
                        j++;
                    }
                    k++;
                }
            }
        }

        private static void AssignBooksToLibraries2()
        {
            SortBooksById();
            Array.Sort(books, (x, y) => x.score.CompareTo(y.score));
            for (int i = 0; i < L; i++)
            {
                libraries[i].booksToScan = new List<int>();
            }
            for (int i = 0; i < B; i++)
            {
                List<library> libsWithThisBook = new List<library>();
                for (int j = 0; j < L; j++)
                {
                    if (libraries[j].capacity - libraries[j].booksToScan.Count > 0)
                    {
                        if (searchfor(i, libraries[j].books) >= 0)
                        {
                            libsWithThisBook.Add(libraries[j]);
                        }
                    }
                }
                if (libsWithThisBook.Count > 0)
                {
                    //var lib = libsWithThisBook.Min((x) => x.nBooks);
                    library lib = FindMin(libsWithThisBook);
                    lib.booksToScan.Add(i);
                    score += books[i].score;
                } 
            }
        }

        private static library FindMin(List<library> libs)
        {
            int ind = 0;
            int min = int.MaxValue;
            for (int i = 0; i < libs.Count; i++)
            {
                if (libs[i].nBooks < min)
                {
                    ind = i;
                    min = libs[i].nBooks;
                }
            }
            return libs[ind];
        }

        private static void ComputeBooksPerLibrary()
        {
            for (int i = 0; i < L; i++)
                if (D - libraries[i].startTime <= 0)
                    libraries[i].capacity = 0;
                else if ((D - libraries[i].startTime) * libraries[i].booksPerDay < 0)
                    libraries[i].capacity = libraries[i].nBooks;
                else
                    libraries[i].capacity = Math.Min((D - libraries[i].startTime) * libraries[i].booksPerDay, libraries[i].nBooks);
        }

        private static void ComputePreliminaryCapacity()
        {
            for (int i = 0; i < L; i++)
                if (D - libraries[i].signupTime <= 0)
                    libraries[i].preliminaryCapacity = 0;
                else if ((D - libraries[i].signupTime) * libraries[i].booksPerDay < 0)
                    libraries[i].preliminaryCapacity = libraries[i].nBooks;
                else
                    libraries[i].preliminaryCapacity = Math.Min((D - libraries[i].signupTime) * libraries[i].booksPerDay, libraries[i].nBooks);
        }

        private static void ComputeStartTimes()
        {
            int startDay = 0;
            for (int i = 0; i < L; i++)
            {
                startDay += libraries[libraryOrder[i]].signupTime;
                libraries[libraryOrder[i]].startTime = startDay;
            }
        }



        private static void OrderLibraries()
        {
            double[] ranking = new double[L];
            for (int i = 0; i < L; i++)
            {
                libraryOrder[i] = i;
                //ranking[i]= libraries[i].booksPerDay * libraries[i].nBooks;
                ranking[i] = ComputeRanking(i);
            }
            Array.Sort(ranking, libraryOrder);
            //Array.Reverse(libraryOrder);
        }

        private static double ComputeRanking(int i)
        {
            return libraries[i].signupTime;// / Math.Pow(libraries[i].signupTime, param);
            //return libraries[i].booksPerDay * libraries[i].nBooks;
        }

        private static int nnz()
        {
            int ret = 0;
            for (int i = 0; i < L; i++)
                if (libraries[i].booksToScan.Count > 0)
                    ret++;
            return ret;
        }

        private static List<int[]> GenerateOutput()
        {
            var ret = new List<int[]>();
            int nLibsNonZero = nnz();
            ret.Add(new int[] { nLibsNonZero });
            for (int i = 0; i < L; i++)
            {
                var lib = libraries[libraryOrder[i]];
                if (lib.booksToScan.Count > 0)
                {
                    ret.Add(new int[] { libraryOrder[i], lib.booksToScan.Count });
                    ret.Add(lib.booksToScan.ToArray());
                }
            }
            return ret;
        }


        private static void ParseList(List<int[]> input)
        {
            B = input[0][0];
            L = input[0][1];
            D = input[0][2];
            books = new book[B];
            for (int i = 0; i < B; i++)
            {
                books[i] = new book();
                books[i].id = i;
                books[i].score = input[1][i];
                books[i].isScanned = false;
            }
            int lineNo = 2;
            libraries = new library[L];
            libraryOrder = new int[L];
            for (int i = 0; i < L; i++)
            {
                libraries[i] = new library();
                libraries[i].nBooks = input[lineNo][0];
                libraries[i].signupTime = input[lineNo][1];
                libraries[i].booksPerDay = input[lineNo][2];
                lineNo++;
                libraries[i].books = new book[libraries[i].nBooks];
                for (int j = 0; j < libraries[i].nBooks; j++)
                {
                    int bookNo = input[lineNo][j];
                    libraries[i].books[j] = books[bookNo];
                }
                lineNo++;
            }
        }

        public static int Evaluate()
        {
            return score;
        }

        public static int doSomething()
        {
            return 1;
        }

        static int searchfor(int a, book[] bks, int start, int end)
        {
            int origstart = start;
            var next = start + (end - start) / 2;
            int cur;
            do
            {
                cur = next;
                if (bks[cur].id < a)
                {
                    next = cur + (end - cur + 1) / 2;
                    start = cur + 1;
                }
                else if (bks[cur].id > a)
                {
                    next = start + (cur - start) / 2;
                    end = cur - 1;
                }
            } while (next != cur);
            if (bks[cur].id != a)
                cur = -1;
            return cur;
        }
        static int searchfor(int a, book[] bk)
        {
            return searchfor(a, bk, 0, bk.Length - 1);
        }
    }
}
