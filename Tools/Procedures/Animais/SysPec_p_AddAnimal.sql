declare @o varchar(200); set @o = 'SysPec_p_AddAnimal';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAnimal(
	@xml xml,
	@animal int output
) with encryption as
begin

	begin try

		insert into Animais 
		(
			Codigo,
			Lote,
			NascidoEm,
			Raca,
			Sexo,
			Peso
		)
		select
			null,
			x.n.value('Lote[1]', 'int'),
			x.n.value('NascidoEm[1]', 'datetime'),
			x.n.value('Raca[1]', 'int'),
			x.n.value('Sexo[1]', 'int'),
			x.n.value('Peso[1]', 'float')
		from
			@xml.nodes('/*[1]') x(n);

		set @animal = scope_identity();

		declare @IdLote int;
				select @IdLote = x.n.value('Lote[1]', 'int') from @xml.nodes('/*[1]') x(n);

		declare @IdFazenda int;
				select @IdFazenda = Fazenda from Lotes where Id = @IdLote

		declare @abreviatura varchar(3);
				select  top 1 @abreviatura =  Abreviatura from Fazendas where Id = @IdFazenda
									
				update Animais 
					set 
						Codigo = @abreviatura + convert(nvarchar(23),Id)
					where 
						Codigo is null and 
						Lote = @IdLote

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go