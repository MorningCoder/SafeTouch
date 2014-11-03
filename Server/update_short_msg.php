<?php
//处理手机客户端上传的短信数据
if(!isset($_POST['username'])||!isset($_POST['sender_num'])||
	!isset($_POST['content'])||!isset($_POST['receive_time']))	
	{
		$error_msg = array();
			$error_msg['result'] = '-1';
				$error_msg['message'] = '信息不完整，上传失败！';
					echo json_encode($error_msg);
						exit();
						}

						$username = $_POST['username'];
						$sender_num = $_POST['sender_num'];
						$content = $_POST['content'];
						$receive_time = $_POST['receive_time'];

						$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);

						if(!$link) 
						{
							$error_msg = array();
							    $error_msg['result'] = '-1';
								    $error_msg['message'] = "连接失败！原因： " . mysql_error();
									    echo json_encode($error_msg);
										}

										mysql_query("set names utf8",$link);

										if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
										{
										    $error_msg = array();
											    $error_msg['result'] = '-1';
												    $error_msg['message'] = "数据库选择失败！原因： " . mysql_error();
													    echo json_encode($error_msg);
														}

														$sql = "insert into ShortMessage values ('$username','$sender_num','$content','$receive_time')";

														if($res = mysql_query($sql,$link))
														{
															$error_msg = array('result' => '0','message' => '上传成功');
																echo json_encode($error_msg);
																}
																else
																{
																	$error_msg = array('result' => '-1','message' => '上传失败！原因：'.mysql_error());
																	    echo json_encode($error_msg);
																		}
																		?>
