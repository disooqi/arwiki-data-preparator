using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace wiki_data_preparator
{
    class arabic_stemmers
    {
        List<string> stopwords = new List<string>();

        string[] Larkey_defarticles = { "ال", "وال", "بال", "كال", "فال", "لل" };
        string[] Larkey_suffixes = {"ها","ان","ات","ون","ين","يه","ية","ه","ة","ي"};
        string[] stop_words = {
          "ان","بعد", "ضد", "يلي", "الى", "في", "من", "حتى", "وهو", "يكون",
        "به", "وليس", "أحد", "على", "وكان", "تلك", "كذلك", "التي", "وبين",
        "فيها", "عليها", "إن", "وعلى", "لكن", "عن", "مساء", "ليس", "منذ",
        "الذي", "أما", "حين", "ومن", "لا", "ليسب", "وكانت", "أي", "ما", "عنه",
        "حول", "دون", "مع", "لكنه", "ولكن", "له", "هذا", "والتي","فقط", "ثم",
        "هذه", "أنه", "تكون", "قد", "بين", "جدا", "لن", "نحو", "كان", "لهم",
        "لأن", "اليوم", "لم", "هؤلاء", "فإن", "فيه", "ذلك", "لو", "عند",
        "اللذين", "كل", "بد", "لدى", "وثي", "أن", "ومع", "فقد", "بل", "هو",
        "عنها", "منه", "بها", "وفي", "فهو", "تحت", "لها", "أو", "إذ", "علي",
        "عليه", "كما", "كيف", "هنا", "وقد", "كانت", "لذلك", "أمام", "هناك",
        "قبل", "معه", "يوم", "منها", "إلى", "إذا", "هل", "حيث", "هي", "اذا",
        "او", "و", "ما", "لا", "الي", "إلي", "مازال", "لازال", "لايزال",
        "مايزال", "اصبح", "أصبح", "أمسى", "امسى", "أضحى", "اضحى", "ظل",
        "مابرح", "مافتئ", "ماانفك", "بات", "صار", "ليس", "إن", "كأن",
        "ليت", "لعل", "لاسيما", "ولايزال", "الحالي", "ضمن", "اول", "وله",
        "ذات", "اي", "بدلا", "اليها", "انه", "الذين", "فانه", "وان",
        "والذي", "وهذا", "لهذا", "الا", "فكان", "ستكون", "مما", "أبو",
        "بإن", "الذي", "اليه", "يمكن", "بهذا", "لدي", "وأن", "وهي", "وأبو",
        "آل", "الذي", "هن", "الذى", null };
        
        public arabic_stemmers()
        {
            foreach (string stopword in stop_words)
            {
                stopwords.Add(stopword);
            }
        }
        public bool is_stopword(string word)
        {
            return stopwords.Contains(word);
        }
        public string normalization(string term,int normalizer_id)
        {
            if (normalizer_id == 0)
                return unified_normalization(term);
            else return term;
        }
        string unified_normalization(string term)
        {
            term = Regex.Replace(term, "[أإآ]", "ا");
            term = Regex.Replace(term, @"[^\w\s]", "");
            term = term.Replace('ى', 'ي');
            term = term.Replace('ة', 'ه');
            term = term.Replace("ـ", string.Empty);
            return term;
        }

        public string stemming(string term, int stemmer_id)
        {
            List<string> tokens = new List<string>();
            tokens.AddRange(term.Split());
            term = string.Empty;

            for (int x = 0; x < tokens.Count; x++)
            {
                if (stemmer_id == 0)
                    tokens[x] = simple_stemmer(tokens[x]);
                else if (stemmer_id == 1)
                    tokens[x] = light10_stemmer(tokens[x]);
                else
                    return term;
                term += tokens[x]+" ";
            }
            return term.Trim();
        }

        string simple_stemmer(string term)
        {
            if (term.StartsWith("وال") && term.Length > 5)
                term = term.Remove(0,3);
            if (term.StartsWith("بال") && term.Length > 5)
                term = term.Remove(0, 3);

            if (term.StartsWith("ال") && term.Length > 4)
                term = term.Remove(0, 2);
            if (term.StartsWith("لل") && term.Length > 4)
                term = term.Remove(0, 2);

            return term;
        }

        string light10_stemmer(string word)
        {
            if (word.Length > 3 && word.StartsWith("و"))
            {
                word = word.Remove(0, 1);
            }

            int len = 0;
            int wordlen = word.Length;

            foreach (string article in Larkey_defarticles)
            {
                if (wordlen > (len = article.Length) + 1 && word.StartsWith(article))
                {
                    word = word.Remove(0, len);
                    break;
                }
            }

            if (word.Length > 2)
            {
                int suflen;
                wordlen = word.Length;

                if (wordlen == 0)
                    return null;

                foreach (string suffix in Larkey_suffixes)
                {
                    if (wordlen > (suflen = suffix.Length) + 1 && word.EndsWith(suffix))
                    {
                        word = word.Remove(wordlen - suflen);
                        wordlen = word.Length;
                    }
                }
            }
            return word;
        }

    }
}
