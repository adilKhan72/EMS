import React, { Component } from "react";
import { styling } from "../PopUpMsgModal/stylepopup.css";
import { Modal, Button, Form } from "react-bootstrap";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
class ExportToPDFRefNumber extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <>
        <Modal
          id="ems_exp_pdf"
          show={this.props.show}
          onHide={this.props.onHide}
          backdrop={"static"}
          keyboard={"false"}
          className={styling}
          animation={false}
          centered
        >
          <div
            className="modal-content"
            style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>Export to PDF</Modal.Title>
            </Modal.Header>
            <Modal.Body>
              {" "}
              <div className="modal-body p-0 material_style">
                <div className="row">
                  <div className="col-12">
                    <div
                      className="card"
                      style={{
                        maxHeight: "400px",
                        boxShadow: "none",
                        border: "0px",
                      }}
                    >
                      <div
                        className="card-body"
                        style={{
                          maxHeight: "400px",
                          overflowY: "scroll",
                          padding: "0rem 1rem",
                        }}
                      >
                        <TableContainer
                          className="table-responsive"
                          component={"div"}
                        >
                          <Table className="table">
                            <TableHead>
                              <TableRow>
                                <TableCell
                                  className="text-left"
                                  style={{ width: "50%" }}
                                >
                                  Project Name
                                </TableCell>
                                <TableCell
                                  className="text-center"
                                  style={{ width: "30%" }}
                                >
                                  Ref. Number
                                </TableCell>
                                {/* <TableCell
                                  className="text-right"
                                  style={{ width: "20%" }}
                                >
                                  Status
                                </TableCell> */}
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {this.props.ExportCheck == true
                                ? this.props.GroupByProject != null
                                  ? this.props.GroupByProject.map(
                                      (item, index) => {
                                        if (item.name != null) {
                                          if (
                                            this.props.ClientIDSelected &&
                                            this.props.ClientIDSelected ==
                                              item.ClientID
                                          ) {
                                            return (
                                              <TableRow>
                                                <TableCell className="text-left">
                                                  {" "}
                                                  {item.name}
                                                </TableCell>
                                                <TableCell className="text-center">
                                                  {" "}
                                                  <input
                                                    maxLength={9}
                                                    className={"form-control"}
                                                    type="text"
                                                    name="Reference_Number"
                                                    // value={this.props.pdftextboxvalue}
                                                    onChange={(e) => {
                                                      this.props.pdftextboxvalueChangedGroupBy(
                                                        e,
                                                        item.ClientID
                                                      );
                                                    }}
                                                    autoComplete="off"
                                                  />
                                                </TableCell>
                                                {/* <TableCell className="text-right">
                                          {" "}
                                          {item.IsActive == 1
                                            ? "Active"
                                            : "In-Active"}
                                        </TableCell> */}
                                              </TableRow>
                                            );
                                          }
                                          if (!this.props.ClientIDSelected) {
                                            return (
                                              <TableRow>
                                                <TableCell className="text-left">
                                                  {" "}
                                                  {item.name}
                                                </TableCell>
                                                <TableCell className="text-center">
                                                  {" "}
                                                  <input
                                                    maxLength={9}
                                                    className={"form-control"}
                                                    type="text"
                                                    name="Reference_Number"
                                                    // value={this.props.pdftextboxvalue}
                                                    onChange={(e) => {
                                                      this.props.pdftextboxvalueChangedGroupBy(
                                                        e,
                                                        item.ClientID
                                                      );
                                                    }}
                                                    autoComplete="off"
                                                  />
                                                </TableCell>
                                                {/* <TableCell className="text-right">
                                          {" "}
                                          {item.IsActive == 1
                                            ? "Active"
                                            : "In-Active"}
                                        </TableCell> */}
                                              </TableRow>
                                            );
                                          }
                                        }
                                      }
                                    )
                                  : null
                                : this.props.ProjectList != null
                                ? this.props.ProjectList.map((item, index) => {
                                    if (item.ProjectIdAll != null) {
                                      return (
                                        <TableRow>
                                          <TableCell className="text-left">
                                            {" "}
                                            {item.ProjectNameAll}
                                          </TableCell>
                                          <TableCell className="text-center">
                                            {" "}
                                            <input
                                              maxLength={9}
                                              className={"form-control"}
                                              type="text"
                                              name="Reference_Number"
                                              // value={this.props.pdftextboxvalue}
                                              onChange={(e) => {
                                                this.props.pdftextboxvalueChanged(
                                                  e,
                                                  index
                                                );
                                              }}
                                              autoComplete="off"
                                            />
                                          </TableCell>
                                          {/* <TableCell className="text-right">
                                        {" "}
                                        {item.IsActive == 1
                                          ? "Active"
                                          : "In-Active"}
                                      </TableCell> */}
                                        </TableRow>
                                      );
                                    }
                                  })
                                : null}
                            </TableBody>
                          </Table>
                          {/*    <TablePagination
                      component="div"
                      count={this.state.lstUser.length}
                      page={this.state.page}
                      onChangePage={this.handleChangePage}
                      rowsPerPage={this.state.rowsPerPage}
                      onChangeRowsPerPage={this.handleChangeRowsPerPage}
                    /> */}
                        </TableContainer>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              {/* <div className="modal-body material_style">
                <div className="row">
                  <div className="col-md-12">
                    <div className="form-group mb-0">
                      <input
                        //className="form-control"
                        maxLength={9}
                        className={"form-control"}
                        type="text"
                        name="UserName"
                        value={this.props.pdftextboxvalue}
                        onChange={this.props.pdftextboxvalueChanged}
                        autoComplete="off"
                      />
                      <label>Reference Number</label>
                    </div>
                  </div>
                </div>
              </div> */}
            </Modal.Body>
            <Modal.Footer>
              <Button className="btn-black" onClick={this.props.onSubmit}>
                Export
              </Button>
            </Modal.Footer>
          </div>
        </Modal>
      </>
    );
  }
}

export default ExportToPDFRefNumber;
