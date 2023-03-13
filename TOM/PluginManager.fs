module TOM.PluginManager

open System
open System.Reflection
open System.Linq
open Microsoft.FSharp.Linq

type IService =
    interface
    end

type PluginManager(pluginRoots: string list) =
    let rec recurseAssemblySearch(rootName) =
        System.IO.Directory.GetDirectories(rootName)
        |> Seq.fold (fun fileList dirName -> (recurseAssemblySearch dirName) @ fileList) list.Empty
        |> fun sublist -> System.IO.Directory.GetFiles(rootName, "*.dll")
                          |> Seq.toList |> fun lst -> lst @ sublist 

    let assemblyList =
        pluginRoots
        |> Seq.fold (fun assemblyList dirname -> (recurseAssemblySearch dirname) @ assemblyList) list.Empty
        |> Seq.map (fun dirName -> Assembly.Load(dirName))
        
    let GetPublicTypesImplementing intFace (assembly:Assembly) =
        assembly.ExportedTypes
        |> Seq.fold (fun typList typ ->
                typ.GetInterfaces().Contains(intFace)
                |> function
                    | true -> typ::typList
                    | false -> typList
            ) List.empty
    let services =
        assemblyList
        |> Seq.fold (fun svcMap assembly ->
                GetPublicTypesImplementing typedefof<IService> assembly
                |> Seq.fold (fun svcMap atype ->
                        atype.GetInterfaces()
                        |> Seq.fold (fun iface ->
                            svcMap
                            |> Map.change iface (fun tlist ->
                                match tlist with
                                    |Some oldlist -> Some(atype::oldlist)
                                    |None -> Some(atype::List.Empty)
                                )
                            )
                        ) svcMap
                ) Map.empty
