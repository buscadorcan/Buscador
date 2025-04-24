CREATE   VIEW vw_EventUserAll AS
        SELECT  'Usuario CAN' AS    vw_Text  ,'vw_EventUserCAN' AS vw_EventUser
UNION   SELECT  'Usuario ONA'                   ,'vw_EventUserONA'
UNION   SELECT  'Usuario de  Consulta'          ,'vw_EventUserREAD'
UNION   SELECT  'Usuario del Buscador'          ,'vw_EventUserSEARCH';
