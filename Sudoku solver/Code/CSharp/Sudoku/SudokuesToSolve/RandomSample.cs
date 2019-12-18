using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuesToSolve
{
    public class RandomSample
    {
        private Random _random;

        public RandomSample()
        {
            _random = new Random((int)(DateTime.Now.Ticks % 64765L));
        }

        public ArrayList ReturnRandomSample(ArrayList v, int sampleSize)
        {
            int i, index, indexUp, nStepsUp, indexDown, nStepsDown, n = v.Count;
            bool[] indexIsTaken;
            ArrayList a;

            if (sampleSize > n)
            {
                throw new Exception("Sample size must be less than target size");
            }

            indexIsTaken = new bool[n];

            for(i = 0; i < n; i++)
            {
                indexIsTaken[i] = false;
            }

            a = new ArrayList();

            for (i = 0; i < sampleSize; i++)
            {
                index = _random.Next(n);

                if (indexIsTaken[index])
                {
                    nStepsUp = GetIndexUp(index, indexIsTaken, out indexUp);
                    nStepsDown = GetIndexDown(index, indexIsTaken, out indexDown);

                    if (
                        ((nStepsUp != -1) && (nStepsDown == -1)) ||
                        ((nStepsUp != -1) && (nStepsDown != -1) && (nStepsUp < nStepsDown))
                        )
                        index = indexUp;
                    else if (
                        ((nStepsDown != -1) && (nStepsUp == -1)) ||
                        ((nStepsDown != -1) && (nStepsUp != -1) && (nStepsDown < nStepsUp))
                        )
                        index = indexDown;
                    else
                    {
                        if (_random.Next(2) == 0)
                        {
                            index = indexUp;
                        }
                        else
                        {
                            index = indexDown;
                        }
                    }
                }

                indexIsTaken[index] = true;
                a.Add(v[index]);
            }

            return a;
        }

        private int GetIndexUp(int startIndex, bool[] indexIsTaken, out int index)
        {
            int retVal, currentIndex, nStepsUp = 0;

            index = -1;
            retVal = -1;
            currentIndex = startIndex;

            while (index == -1 && ((currentIndex - 1) >= 0))
            {
                nStepsUp++;
                currentIndex--;

                if (!indexIsTaken[currentIndex])
                {
                    index = currentIndex;
                    retVal = nStepsUp;

                }
            }

            return retVal;
        }

        private int GetIndexDown(int startIndex, bool[] indexIsTaken, out int index)
        {
            int retVal, currentIndex, nStepsDown = 0;

            index = -1;
            retVal = -1;
            currentIndex = startIndex;

            while (index == -1 && (currentIndex + 1) < indexIsTaken.Length)
            {
                nStepsDown++;
                currentIndex++;

                if (!indexIsTaken[currentIndex])
                {
                    index = currentIndex;
                    retVal = nStepsDown;

                }
            }

            return retVal;
        }
    }
}
