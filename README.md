# Compass -  Konstruktion mit Zirkel und Lineal

## Kontrollfluss

Ein Programm besteht aus einer Reihe von Befehlen, die durch Kommas, Semikola oder Zeilenumbrüche getrennt werden. Beispiel:

    A(1|1), B(2|4)
    C1 = A°B;
    C2 = B°A;
    {X1, X2} <- C1 n C2;
    {M}      <- (X1-X2) n (C1-C2)

Ein Befehl kann folgende Gestalt besitzen:

1. **Punktdefinition**
      
    P(x-coord|y-coord)

2. **Variablendefiniton**

    VAR = expr

3. *Pick*-**Definition**

    {VAR1, ..., VARN} <- expr

Wird ein Ausdruck mit Komma oder Zeilenumbruch abgeschlossen, so wird *Compass* das Ergebnis auf dem Bildschirm zu zeichnen versuchen. Das Zeichnen wird durch `;` am Ende der Zeile unterdrückt. 

## Ausdrücke

Es exisiteren folgende Operatoren

**Strecken und Geraden durch Punkte**

    L = A-B; 
    G = A--B;

**Kreis mit Mittelpunkt `M` durch Punkt `P`**

    C = M°P;

**Schnittmenge**

    P = A n B

**Subroutine**

    M = proc(A,B)
	  {X,Y} <- (A°B) n (B°A);
      (X-Y) n (A-B)
    end;

**Funktionsaufruf**

    F(A,B)