declare @o varchar(200); set @o = 'SysPec_p_SaveAnimal';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_SaveAnimal(
	@xml xml
) with encryption as
begin

	begin try
		
		update Animais set
			Codigo = x.n.value('Codigo[1]', 'varchar(25)'),
			Lote = x.n.value('Lote[1]', 'int'),
			NascidoEm = x.n.value('NascidoEm[1]', 'datetime'),
			Habilitado = x.n.value('Habilitado[1]', 'bit'),
			Raca = x.n.value('Raca[1]', 'int'),
			Peso = x.n.value('Peso[1]', 'float')
		from
			@xml.nodes('/*[1]') x(n)
		where
			Id = x.n.value('Id[1]', 'int');

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go