#!/usr/bin/perl -w

use warnings;

my %wordnet =(
    "persoane" => qr/(?>realizator|regizor|preşedinte|soţi.|scriitor|actor|artist\s|cântăreţ|istoric|gânditor|autor|muritoa?r|membr.|partener.?|zeiţă|reprezentanţi|dictator|discipol)/,
    "numere" => qr/(?>poziţi.|vârst..?|procent.?.?|număr.?.?)/,
    "masuri" => qr/(?<!(?>cu|pe)\s)(?>suprafaţ.|înălţime.?|mas.|amplitudine|durata|adâncime|vitez.|lungime.?|cantitate.?|dimensiuni|densitatea)/,
    "locatii" => qr/(?>regiun.|judeţ|raioane|localit..|oraş|capital.|insul.|vârf|ţ..?r..?|teritori.|provincie|continent|stat)/,
    "timp" => qr/(?>an|secol|perioad.|dat.|timp|oră|ora)/,
    "organizatii" => qr/(?>organizaţi.|Organizaţi|agenţi.|universitate|grupare|compani.|firm.|entitate.?|fond.?.?|producător|institu.|închisoare|ordin..|partid|CIA|Agenţia...|UNESCO|Eesti|Solidaritatea|Eteria|condus|NATO|Uniunea Europeană)/,
    "obiecte" => qr/(?>un|o)?\s?(?>instrument|produs|armele|aparat|mijloc|unitatea|beton-lemn|modul|echipament|jucări.|busol.|armur.|echipament|calculator|dispozitiv|Selena|patrafir)/
    );

my %a_types = (
    "PERSON" =>      [qr/(?>Ce|Care|Numiţi|Cu cine|Cum se numeşte)(?!\s*$wordnet{"organizatii"}).*?$wordnet{"persoane"}/,
                      qr/Cum (?>o|îl) chea?m./, qr/Cine/, qr/Cui/, qr/Al cui/, qr/Cu cine/],
    "COUNT" =>       [qr/Aproximativ câte/,qr/A câta/, qr/Câte/, qr/Câţi/, qr/Din câte/, qr/(?>Ce|Care este)\s$wordnet{"numere"}/],
    "MEASURE" =>     [qr/Cât\s/, qr/(?>Ce|Care|Sub ce).*?$wordnet{"masuri"}/],
    "LOCATION" =>    [qr/Ce (stat|oraş)/, qr/De unde/,qr/Unde/, qr/(?>.{2,4}\sce|Care|Pe ce|Asupra căror|Numiţi.).*?$wordnet{"locatii"}/],
    "TIME" =>        [qr/Când/, qr/(?>În ce|Din ce|La ce|După câ(?>t|ţi))\s$wordnet{"timp"}/],
    "ORGANIZATION" =>[qr/Cine producea/, qr/(?>Ce (?!a\s)|Care este|La|Din ce|Cum se nume.).*?(?<!al\s)$wordnet{"organizatii"}/],
    "OBJECT" =>      [qr/Ce poartă numele/, qr/Ce (?>(?>s-)?a\s)|dărui/, qr/Cu ce .-a\s/,
                      qr/(?>Ce|Care|Numiţi|Cum se numeşte|Pentru ce|La ce).*?$wordnet{"obiecte"}/]
    );

my %q_types = (
    "DEFINITION" => [qr/Ce înseamnă\s+[A-Z]/,
		     qr/Ce ( )?este (un|o)/,
		     qr/Cine (?>este|(?>a fost))\s+[A-Z]/,
                     qr/Ce (?>este|(?>a fost)|sunt)\s+(?!(?>un|o)\s)/],
    "LIST" =>       [qr/Asupra căror/, qr/Ce tipuri/, qr/Cine erau/, qr/Care sunt/, qr/Cui/, qr/Prin ce/,
                     qr/Numiţi (?!(?>un|o)\s)/]);


my %count = ("all" => 0, "right_q" => 0, "right_a" => 0);
my %pos;
my %lemma;

det_pos_word();
open FILE, '2008.xml' or die "Error: $!";
open FPI, '01_Procesare_intrebare.csv' or die "Error: $!";
@fpi = <FPI>;
close FPI;
open FOUT, ">01_tip_i_r.txt";
$/ = "</q>"; #smart buffer
my $contor = 1;
my $q_type_correct = 0;
my $a_type_correct = 0;

$ne_first_question="";
$old_group="";
$query = "";

while (defined (my $chunk = <FILE>)) {
    if ($chunk =~ m|<q.*?q_group_id=\"([0-9]+)\">(.*)</q>|) {
            $group = $1;
    	    $question = $2;

            if($group ne $old_group)
            {
            	    $old_group = $group;
                    $ne_first_question = "";
            }

            $query = $ne_first_question;

            $a_type = get_type($question, \%a_types, "OTHER");
            $q_type = get_type($question, \%q_types, "FACTOID");

            print $contor , " " , $group , " " , $question."\n";
            print keywords_focus($question),"\n";
            print "Question Type: $q_type\n";
            print "Answer Type: $a_type\n\n";

            if($fpi[$contor] =~ $q_type)
            { $q_type_correct++; }
            else
            { #print "QTYPEPROBLEM ",$contor," ", $question." $q_type\n";
            }

            if($fpi[$contor] =~ $a_type)
            { $a_type_correct++; }
            else
            { #print "ATYPEPROBLEM ",$contor," ", $question." $a_type\n";
            }

            print FOUT $contor++, "$a_type $q_type\n";
    }
}
close FILE;
close FOUT;

print "QTYPE Correct $q_type_correct \nATYPE Correct $a_type_correct \n";
print "Ready";

sub get_type {
    my $question = shift;
    my %hash = %{(shift)};
    my $type = shift;

    KEY:foreach my $key (keys %hash) {
                foreach my $match (@{$hash{$key}}) {
                    if ($question =~ m/$match/) {
                        $type = $key; last KEY;
                    }
                }
            }
    return $type;
}

sub keywords_focus{
	my $question = shift;
        my $resultA = "";
	my $resultN = "";
        my $resultV = "";
        my $resultNE = "";
        my $focus = "";

        if(substr($question,-1,1) eq ".")
        {
              	$question = substr($question,0,length($question)-1)
        }

        $question =~ s/\?//g;
	#$question =~ s/,/ /g;
       	#$question =~ s/\./ /g;
       	$question =~ s/\\/ /g;
        $question =~ s/\"/ /g;
	$question =~ s/\'/ /g;
	$question =~ s/:/ /g;
        #$question =~ s/-/ /g;
	$question =~ s/  / /g;

	foreach $word(split(" ",$question))
        {
        	 if(substr($word,-1,1) eq "," || substr($word,-1,1) eq "-")
                 {
                 	$word = substr($word,0,length($word)-1)
                 }

                 if($word =~ /Luni|Marţi|Miercuri|Joi|Vineri|Sâmbătă|Duminică|Ianuarie|Februarie|Martie|Aprilie|Iunie|Iulie|August|Septembrie|Octombrie|Noiembrie|Decembrie/i)
                 {
        		$resultNE .= $word." ";
                        if($word ne $lemma{$word})
                        { $query .= "(".$word." ".$lemma{$word}."\)\^3 ";}
                        else
                        { $query .= $word . "^3 ";}
                 }
                 elsif(defined($pos{$word}) && $word ne "al" && $word ne "ale" && $word ne "astfel" && $word ne "mai" && $word ne "ai" && $word ne "au" && $word ne "care")
                 {
                 	if($pos{$word} eq "V")
	                {
                                $resultV .= $word." ";
                                if($lemma{$word} ne "fi" && $lemma{$word} ne "avea" && $lemma{$word} ne "însemna")
                                {
		                if($word ne $lemma{$word})
                                {     	$query .= "(".$word."^2 ".$lemma{$word}.") "; }
                                else
                                { $query .= $word . " ";}
                                }
                        }
                      	elsif($pos{$word} eq "N")
	                {
	                	if($lemma{$word} ne "oraş" && $lemma{$word} ne "ţară")
                                {
	                        if ($focus eq "")
        	                {
                                	$focus = $word;
                                }
                                else
                                {
	                                $resultN .= $word." ";
                                }

                                if($word ne $lemma{$word})
                                {$query .= "(".$word."^2 ".$lemma{$word}.") "; }
                                else
                                { $query .= $word . " ";}

                                }
                        }
                      	elsif($pos{$word} eq "A")
	                {
                        	$resultA .= $word." ";
		                if($word ne $lemma{$word})
                                {     	$query .= "(".$word."^2 ".$lemma{$word}.") "; }
                                else
                                { $query .= $word . " ";}
                        }
                 }
                 elsif($word eq "Numiţi")
                 {
                 	$resultV .= lc($word)." ";
                 }
                 elsif($word eq "Buchetul")
                 {
                 	$resultN .= lc($word)." ";
                 }
                 elsif($word =~ /(([0-9]+),([0-9]+))|([0-9]+)|(([A-Z]+|Ţ)[a-zşâîăţ]*)/
                 && $word ne "Ce" && $word ne "Pentru" && $word ne "Cine" && $word ne "Când" && $word ne "Câte"
                 && $word ne "La" && $word ne "Care" && $word ne "Cum" && $word ne "Din" && $word ne "şi"
                 && $word ne "Al" && $word ne "Prin" && $word ne "Cu" && $word ne "Unde" && $word ne "Cât"
                 && $word ne "Pe" && $word ne "Sub" && $word ne "Câţi" && $word ne "După" && $word ne "A" && $word ne "Asupra" && $word ne "Aproximativ" && length($word)>1)
                 {
                         $resultNE .= $word." ";
                         if(defined($lemma{$word}))
                         {
                           $query .= "\(" . $word . " " . $lemma{$word}. "\)\^3 ";
                         }
                         else
                         {
                         $query .= $word."\^3 ";
                         }
                 }
        }

	if($ne_first_question eq "")
        {
              $ne_first_question = $resultNE;
        }
        $resultNE =~ s/\s+$//;
	$query =~ s/\s+$//;

        if($q_type eq "DEFINITION" && $query eq $resultNE."^3")
        {
           $query = "title:$resultNE";
        }

        return "Focus:$focus\nVerb:$resultV\nNoun:$resultN\nAdj:$resultA\nNameEntities:$resultNE\nQuery:$query";
}

sub det_pos_word{
	open FL,"lemme.txt" or die "Lemme not found";
        while(<FL>)
        {
            my $type = substr($_,0,1);
            $lemma_currenta = substr($_,3, index($_," ")-4);
            while(/\"(.*?)\"/g)
            {
            	if(!defined($pos{$1}) || $type eq "V")
                {
                	$pos{$1} = $type;
                        $lemma{$1} = $lemma_currenta;
                }
                elsif($type eq "N" && $pos{$1} eq "A")
                {
                	$pos{$1} = $type;
                        $lemma{$1} = $lemma_currenta;
                }
            }
        }
        close FL;

      	open FL,"NE_lemma.lst" or die "Lemme not found";
        while(<FL>)
        {
            $lemma_currenta = substr($_,0, index($_," "));
	    @words = split;
	    for($i=1;$i<=$#words;$i++)
            {
            		$lemma{$words[$i]} = $lemma_currenta;
            }

        }

        close FL;

}