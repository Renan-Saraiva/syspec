declare @o varchar(200); set @o = 'SysPec_p_ListAnimaisByLoteName';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAnimaisByLoteName(
	@LoteName varchar(150),
	@FazendaId int
) 
with encryption as
begin
	declare @ValorBusca varchar(max);
	set @ValorBusca = '%' + @LoteName + '%';

	select
		Animal.*
	from
		SysPec_c_Animais [Animal]
	where 
		LoteNome like @ValorBusca and Fazenda = @FazendaId
	for xml auto, elements, root('Animais');
end;
go