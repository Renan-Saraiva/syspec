declare @o varchar(200); set @o = 'SysPec_p_GetCriador';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_GetCriador(
	@user nvarchar(256)
) with encryption as
begin

	select
		Id,
		Nome,
		Usuario,
		Telefone
	from
		Criadores [Criador]
	where
		Usuario = @user
	for xml auto, elements;

end
go
