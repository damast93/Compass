Imports System.Runtime.CompilerServices
Imports Compass.Geometry

Namespace Interpreter

    Public Module ValueExtension

        <Extension()>
        Public Function AsOf(Of T As Value)(arg As Value) As T
            Dim x As T = DirectCast(arg, T)
            If x IsNot Nothing Then Return x
            Throw New TypeMismatchException(String.Format("Type {0} cannot be converted into {1}", arg.GetType().Name, GetType(T).Name))
        End Function

    End Module
End Namespace