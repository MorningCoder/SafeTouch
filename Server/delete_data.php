<?php
//该页面用于处理所有数据的删除工作
//主要是操作数据库
if(!isset($_POST['type'])||!isset($_POST['username']))
{
	$error_msg = array('result' => '-1','message' => '操作失败！为选择操作数据类型' );
		echo json_encode($error_msg);
			exit();
			}
			$type = $_POST['type'];
			$username = $_POST['username'];

			//打开数据库连接
			$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
			if(!$link) 
			{
			    $error_msg = array();
				    $error_msg['result']='-1';
					    $error_msg['message']="连接失败！原因： " . mysql_error();

						    echo json_encode($error_msg);
							}
							//设置字符编码
							mysql_query("set names utf8",$link);
							//选择数据库
							if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
							{
							    $error_msg = array();
								    $error_msg['result']='-1';
									    $error_msg['message']="选择数据库失败！原因：" . mysql_error($link);
										    echo json_encode($error_msg);
											}

											switch($type)
											{
												case '1'://代表短信数据
														if(!isset($_POST['sender_number'])||!isset($_POST['receive_time']))
																{
																			$error_msg = array('result' => '-1','message' => '操作指令不完整，操作失败！' );
																						echo json_encode($error_msg);
																									exit();
																											}
																													$sender_number = $_POST['sender_number'];
																															$receive_time = $_POST['receive_time'];

																																	$sql = "delete from ShortMessage where username='$username' and sender_number='$sender_number'
																																				and receive_time='$receive_time'";

																																						$res = mysql_query($sql,$link);
																																								//操作失败则发送失败信息，成功则不发送信息，用于客户端判断返回流
																																										if(!$res)
																																												{
																																															$error_msg = array('result' => '-1', 'message' => '删除失败！原因：'.mysql_error());
																																																		echo json_encode($error_msg);
																																																				}
																																																					break;
																																																						case '2'://代表联系人列表
																																																								if(!isset($_POST['contacter_num']))
																																																										{
																																																													$error_msg = array('result' => '-1','message' => '操作指令不完整，操作失败！' );
																																																																echo json_encode($error_msg);
																																																																			exit();
																																																																					}
																																																																							$contacter_num = $_POST['contacter_num'];

																																																																									$sql = "delete from Contacter where username='$username' and contacter_num='$contacter_num'";

																																																																											$res = mysql_query($sql);
																																																																													//同上
																																																																															if(!$res)
																																																																																	{
																																																																																				$error_msg = array('result' => '-1', 'message' => '删除失败！原因：'.mysql_error());
																																																																																							echo json_encode($error_msg);
																																																																																									}
																																																																																										break;
																																																																																											default:
																																																																																													$error_msg = array('result' => '-1', 'message' => '删除失败，操作指令无法解析');
																																																																																															echo json_encode($error_msg);
																																																																																																break;
																																																																																																}
