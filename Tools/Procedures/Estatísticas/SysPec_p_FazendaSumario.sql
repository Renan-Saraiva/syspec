declare @o varchar(200); set @o = 'SysPec_p_FazendaSumario';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_FazendaSumario(
	@fazenda int,
	@criador int,
	@animais int output,
	@lotes int output,
	@racoes int output,
	@pastos int output
)
with encryption as
begin	
	select @animais = count(1) from syspec_c_animais where Fazenda = @fazenda
	select @lotes = count(1) from Lotes where Fazenda = @fazenda
	select @racoes = count(1) from Racoes where Criador = @criador
	select @pastos = count(1) from Pastos where Fazenda = @fazenda	
end;
go