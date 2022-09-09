// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Version 1.3 - SangYoon Cho
// 2022/09/06    (Write a code for assignment)
//               (Make a two Dictionary stored Dependents and Dependees values for each pairs of the string.)
//               (No throw exceptioni in this code.)
//               (Just return 'nothing' if any changes are not happened.)
// Version 1.4 - SangYoon Cho
// 2022/09/09    (Add a comment and test code.)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private int sizeofDG;
        Dictionary<string, List<string>> Dependents;
        Dictionary<string, List<string>> Dependees;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// Initialize size of DependencyGraph and Dictionary of Dependents and Dependees.
        /// </summary>
        public DependencyGraph()
        {
            sizeofDG = 0;
            Dependents = new Dictionary<string, List<string>>();
            Dependees = new Dictionary<string, List<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return sizeofDG; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a").
        /// If dependees don't have "s" key, it returns 0.
        /// </summary>
        public int this[string s]
        {
            get { return Dependees.ContainsKey(s) ? Dependees[s].Count : 0; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        /// <param name="s"> The key string of Dependents. </param>
        /// <returns> If dependents has 's' key, return true
        ///                                      else false.
        /// </returns>
        public bool HasDependents(string s)
        {
            if (!Dependents.ContainsKey(s))
                return false;
            else
            {
                if (Dependents[s].Count > 0)
                    return true;
                else
                    return false;
            }
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        /// <param name="s"> The key string of dependees. </param>
        /// <returns> If dependees has 's' key and it is not empty, return true
        ///                                                         else false.
        /// </returns>
        public bool HasDependees(string s)
        {
            if (!Dependees.ContainsKey(s))
                return false;
            else
            {
                if (Dependees[s].Count > 0)
                    return true;
                else
                    return false;
            }
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        /// <param name="s"> The key string of dependents </param>
        /// <returns> If dependents has 's' key, return dependents(s)
        ///                                      else return empty List(Enumerator)
        /// </returns>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!Dependents.ContainsKey(s))
                return new List<string>();
            return Dependents[s];
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        /// <param name="s"> The key string of dependents </param>
        /// <returns> If dependents has 's' key, return dependents(s)
        ///                                      else return empty List(Enumerator)
        /// </returns>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!Dependees.ContainsKey(s))
                return new List<string>();
            return Dependees[s];
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            List<string> inDepents = new List<string>();
            List<string> inDees = new List<string>();

            // If s,t already exist in the dependents
            if (Dependents.ContainsKey(s) && Dependents[s].Contains(t))
            {
                return;
            }
            else
            {
                // if Depedents contain 's'
                if (Dependents.ContainsKey(s))
                {
                    // if dependees contain 't'
                    if (Dependees.ContainsKey(t))
                    {
                        Dependents[s].Add(t);
                        Dependees[t].Add(s);
                    }
                    else
                    {
                        inDees.Add(s);
                        Dependees.Add(t, inDees);

                        Dependents[s].Add(t);
                    }
                }
                // if Dependents doesn't contain 's' key.
                else
                {
                    // if dependees contain 't'
                    if (Dependees.ContainsKey(t))
                    {
                        inDepents.Add(t);
                        Dependents.Add(s, inDepents);

                        Dependees[t].Add(s);
                    }
                    else
                    {
                        inDepents.Add(t);
                        inDees.Add(s);

                        Dependents.Add(s, inDepents);
                        Dependees.Add(t, inDees);
                    }
                }
                sizeofDG++;
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// If dependents or dependees is empty, it removes nothing.
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S </param>
        /// <param name="t"> t cannot be evaluated until s is </param>
        public void RemoveDependency(string s, string t)
        {
            if (Dependents.ContainsKey(s) && Dependents[s].Contains(t))
            {
                Dependents[s].Remove(t);
                Dependees[t].Remove(s);
                sizeofDG--;
            }
            else
                return;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // if dependents doesn't contain 's'
            if (!Dependents.ContainsKey(s))
            {
                if (newDependents.Count() == 0)
                    // make an empty enumerate dependees.
                    Dependents.Add(s, new List<string>());
                else
                    foreach (string dents in newDependents)
                        AddDependency(s, dents);
            }
            else
            {
                foreach (string dents in Dependents[s].ToList())
                    RemoveDependency(s, dents);
                foreach (string dents in newDependents)
                    AddDependency(s, dents);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // if dependents doesn't contain 's'
            if (!Dependees.ContainsKey(s))
            {
                // make an empty enumerate dependents.
                if (newDependees.Count() == 0)
                    Dependees.Add(s, new List<string>());
                else
                    foreach (string dees in newDependees)
                        AddDependency(dees, s);
            }
            else
            {
                foreach (string dees in Dependees[s].ToList())
                    RemoveDependency(dees, s);
                foreach (string dees in newDependees)
                    AddDependency(dees, s);
            }
        }
    }
}
